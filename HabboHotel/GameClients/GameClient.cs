using System;

using Quasar.Net;
using System.Linq;
using Quasar.Core;
using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Interfaces;
using Quasar.HabboHotel.Users.UserDataManagement;
using ConnectionManager;

using Quasar.Communication.Packets.Outgoing.Sound;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.Communication.Packets.Outgoing.Handshake;
using Quasar.Communication.Packets.Outgoing.Navigator;
using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Quasar.Communication.Packets.Outgoing.Inventory.Achievements;


using Quasar.Communication.Encryption.Crypto.Prng;
using Quasar.HabboHotel.Users.Messenger.FriendBar;
using Quasar.Communication.Packets.Outgoing.BuildersClub;
using Quasar.HabboHotel.Moderation;

using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Subscriptions;
using Quasar.HabboHotel.Permissions;
using Quasar.Communication.Packets.Outgoing;
using Quasar.Communication.Packets.Outgoing.Nux;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using System.Collections.Generic;
using Quasar.HabboHotel.Catalog;
using Quasar.Communication.Packets.Outgoing.Talents;
using Quasar.HabboHotel.Users.Messenger;
using System.Xml;
using System.Net;
using System.IO;
using Quasar.Communication.Packets.Outgoing.Campaigns;
using Quasar.Communication.Packets.Outgoing.Notifications;
using System.Threading;
using Quasar.Communication.Packets.Outgoing.Messenger;

namespace Quasar.HabboHotel.GameClients
{
    public class GameClient
    {
        private readonly int _id;
        private Habbo _habbo;
        public string MachineId;
        private bool _disconnected;
        public ARC4 RC4Client = null;
        private GamePacketParser _packetParser;
        private ConnectionInformation _connection;
        public int PingCount { get; set; }

        public GameClient(int ClientId, ConnectionInformation pConnection)
        {
            this._id = ClientId;
            this._connection = pConnection;
            this._packetParser = new GamePacketParser(this);
            this.PingCount = 0;
        }

        private void SwitchParserRequest()
        {
            _packetParser.SetConnection(_connection);
            _packetParser.onNewPacket += parser_onNewPacket;
            byte[] data = (_connection.parser as InitialPacketParser).currentData;
            _connection.parser.Dispose();
            _connection.parser = _packetParser;
            _connection.parser.handlePacketData(data);
        }

        private void parser_onNewPacket(ClientPacket Message)
        {
            try
            {
                QuasarEnvironment.GetGame().GetPacketManager().TryExecutePacket(this, Message);
            }
            catch (Exception e)
            {
                Logging.LogPacketException(Message.ToString(), e.ToString());
            }
        }

        internal string SendNotification(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        private void PolicyRequest()
        {
            _connection.SendData(QuasarEnvironment.GetDefaultEncoding().GetBytes("<?xml version=\"1.0\"?>\r\n" +
                   "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                   "<cross-domain-policy>\r\n" +
                   "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                   "</cross-domain-policy>\x0"));
        }


        public void StartConnection()
        {
            if (_connection == null)
                return;

            this.PingCount = 0;

            (_connection.parser as InitialPacketParser).PolicyRequest += PolicyRequest;
            (_connection.parser as InitialPacketParser).SwitchParserRequest += SwitchParserRequest;
            _connection.startPacketProcessing();
        }

        public bool TryAuthenticate(string AuthTicket)
        {
            try
            {
                byte errorCode = 0;
                UserData userData = UserDataFactory.GetUserData(AuthTicket, out errorCode);
                if (errorCode == 1 || errorCode == 2)
                {
                    Disconnect();
                    return false;
                }

                #region Ban Checking
                //Let's have a quick search for a ban before we successfully authenticate..
                ModerationBan BanRecord = null;
                if (!string.IsNullOrEmpty(MachineId))
                {
                    if (QuasarEnvironment.GetGame().GetModerationManager().IsBanned(MachineId, out BanRecord))
                    {
                        if (QuasarEnvironment.GetGame().GetModerationManager().MachineBanCheck(MachineId))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }

                if (userData.user != null)
                {
                    //Now let us check for a username ban record..
                    BanRecord = null;
                    if (QuasarEnvironment.GetGame().GetModerationManager().IsBanned(userData.user.Username, out BanRecord))
                    {
                        if (QuasarEnvironment.GetGame().GetModerationManager().UsernameBanCheck(userData.user.Username))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }
                #endregion

                QuasarEnvironment.GetGame().GetClientManager().RegisterClient(this, userData.userID, userData.user.Username);
                _habbo = userData.user;


                if (_habbo != null)
                {
                    userData.user.Init(this, userData);

                    SendMessage(new AuthenticationOKComposer());
                    SendMessage(new AvatarEffectsComposer(_habbo.Effects().GetAllEffects));
                    SendMessage(new NavigatorSettingsComposer(_habbo.HomeRoom));
                    SendMessage(new FavouritesComposer(userData.user.FavoriteRooms));
                    SendMessage(new FigureSetIdsComposer(_habbo.GetClothing().GetClothingAllParts));
                    SendMessage(new UserRightsComposer(_habbo));
                    SendMessage(new AvailabilityStatusComposer());
                    SendMessage(new AchievementScoreComposer(_habbo.GetStats().AchievementPoints));


                    var habboClubSubscription = new ServerPacket(ServerPacketHeader.HabboClubSubscriptionComposer);
                    habboClubSubscription.WriteString("club_habbo");
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(2);
                    habboClubSubscription.WriteBoolean(false);
                    habboClubSubscription.WriteBoolean(false);
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(0);
                    SendMessage(habboClubSubscription);

                    SendMessage(new BuildersClubMembershipComposer());
                    SendMessage(new CfhTopicsInitComposer());

                    SendMessage(new BadgeDefinitionsComposer(QuasarEnvironment.GetGame().GetAchievementManager()._achievements));
                    SendMessage(new SoundSettingsComposer(_habbo.ClientVolume, _habbo.ChatPreference, _habbo.AllowMessengerInvites, _habbo.FocusPreference, FriendBarStateUtility.GetInt(_habbo.FriendbarState)));
                    SendMessage(new TalentTrackLevelComposer());
                    if (GetHabbo().GetMessenger() != null)
                        GetHabbo().GetMessenger().OnStatusChanged(true);

                    if (_habbo.Rank < 2 && !QuasarStaticGameSettings.HotelOpenForUsers)
                    {
                        SendMessage(new SendHotelAlertLinkEventComposer("Actualmente solo el Equipo Adminsitrativo puede entrar al hotel para comprobar que todo está bien antes de que los usuarios puedan entrar. Vuelve a intentarlo en unos minutos, podrás encontrar más información en nuestro Facebook.", QuasarEnvironment.GetDBConfig().DBData["facebook_url"]));
                        Thread.Sleep(10000);
                        Disconnect();
                        return false;
                    }

                    if (!string.IsNullOrEmpty(MachineId))
                    {
                        if (this._habbo.MachineId != MachineId)
                        {
                            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `users` SET `machine_id` = @MachineId WHERE `id` = @id LIMIT 1");
                                dbClient.AddParameter("MachineId", MachineId);
                                dbClient.AddParameter("id", _habbo.Id);
                                dbClient.RunQuery();
                            }
                        }

                        _habbo.MachineId = MachineId;
                    }
                    PermissionGroup PermissionGroup = null;
                    if (QuasarEnvironment.GetGame().GetPermissionManager().TryGetGroup(_habbo.Rank, out PermissionGroup))
                    {
                        if (!String.IsNullOrEmpty(PermissionGroup.Badge))
                            if (!_habbo.GetBadgeComponent().HasBadge(PermissionGroup.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(PermissionGroup.Badge, true, this);
                    }

                    SubscriptionData SubData = null;
                    if (QuasarEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(this._habbo.VIPRank, out SubData))
                    {
                        if (!String.IsNullOrEmpty(SubData.Badge))
                        {
                            if (!_habbo.GetBadgeComponent().HasBadge(SubData.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(SubData.Badge, true, this);
                        }
                    }
                    if (!QuasarEnvironment.GetGame().GetCacheManager().ContainsUser(_habbo.Id))
                        QuasarEnvironment.GetGame().GetCacheManager().GenerateUser(_habbo.Id);

                    _habbo.InitProcess();

                    this.GetHabbo()._lastitems = new Dictionary<int, CatalogItem>();
                    //ICollection<MessengerBuddy> Friends = new List<MessengerBuddy>();
                    //foreach (MessengerBuddy Buddy in this.GetHabbo().GetMessenger().GetFriends().ToList())
                    //{
                    //    if (Buddy == null)
                    //        continue;

                    //    GameClient Friend = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(Buddy.Id);
                    //    if (Friend == null)
                    //        continue;
                    //    string figure = this.GetHabbo().Look;


                    //    Friend.SendMessage(RoomNotificationComposer.SendBubble("usr/look/" + this.GetHabbo().Username + "", this.GetHabbo().Username + " se ha conectado a " + QuasarEnvironment.GetDBConfig().DBData["hotel.name"] + ".", ""));

                    //}

                    //if (GetHabbo()._NUX) { SendMessage(new MassEventComposer("habbopages/bienvenidanormal.txt")); }
                    //else { SendMessage(new MassEventComposer("habbopages/bienvenidauser.txt")); }


                    if (QuasarEnvironment.GetDBConfig().DBData["pin.system.enable"] == "0")
                        GetHabbo().StaffOk = true;

                    if(GetHabbo().StaffOk)
                    {
                        if (GetHabbo().GetPermissions().HasRight("mod_tickets"))
                        {
                              SendMessage(new ModeratorInitComposer(
                              QuasarEnvironment.GetGame().GetModerationManager().UserMessagePresets,
                              QuasarEnvironment.GetGame().GetModerationManager().RoomMessagePresets,
                              QuasarEnvironment.GetGame().GetModerationManager().GetTickets));
                        }
                    }
                    
                     if (GetHabbo().Rank >= 3)
                    {
                        using (IQueryAdapter dbClient = OreoServer.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `ranks` WHERE id = '" + GetHabbo().Rank + "'");
                            DataRow Table = dbClient.getRow();

                            if (!GetHabbo().GetBadgeComponent().HasBadge(Convert.ToString(Table["badgeid"])))
                            {

                            }
                            else
                            {

                                GetHabbo().GetBadgeComponent().GiveBadge(Convert.ToString(Table["badgeid"]), true, GetHabbo().GetClient());
                                SendMessage(RoomNotificationComposer.SendBubble("badge/" + Table["badgeid"], "Você recebeu o emblema staff de acordo com seu rank!", "/inventory/open/badge"));
                            }
                        }
                    }


                    if (QuasarEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer != null)
                    {
                        QuasarEnvironment.GetGame().GetTargetedOffersManager().Initialize(QuasarEnvironment.GetDatabaseManager().GetQueryReactor());
                        TargetedOffers TargetedOffer = QuasarEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer;

                        if (TargetedOffer.Expire > QuasarEnvironment.GetIUnixTimestamp())
                        {

                            if (TargetedOffer.Limit != GetHabbo()._TargetedBuy)
                            {

                                SendMessage(QuasarEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer.Serialize());
                            }
                        }
                        else if (TargetedOffer.Expire == -1)
                        {

                            if (TargetedOffer.Limit != GetHabbo()._TargetedBuy)
                            {

                                SendMessage(QuasarEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer.Serialize());
                            }
                        }
                        else
                        {
                            using (var dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                dbClient.runFastQuery("UPDATE targeted_offers SET active = 'false'");
                            using (var dbClient2 = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                dbClient2.runFastQuery("UPDATE users SET targeted_buy = '0' WHERE targeted_buy > 0");
                        }
                    }

                    SendMessage(new HCGiftsAlertComposer());

                    QuasarEnvironment.GetGame().GetRewardManager().CheckRewards(this);

                    if (GetHabbo()._NUX)
                    {
                        var nuxStatus = new ServerPacket(ServerPacketHeader.NuxUserStatus);
                        nuxStatus.WriteInteger(2);
                        SendMessage(nuxStatus);
                        QuasarEnvironment.GetGame().GetClientManager().StaffAlert(new RoomInviteComposer(int.MinValue, GetHabbo().Username + " acaba de registrarse en " + QuasarEnvironment.GetDBConfig().DBData["hotel.name"]));
                        SendMessage(new NuxAlertComposer("nux/lobbyoffer/hide"));
                        SendMessage(new NuxAlertComposer("helpBubble/add/HC_JOIN_BUTTON/Como puedes apreciar dispones de Habbo Club infinito."));
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Logging.LogCriticalException("Bug during user login: " + e);
            }
            return false;
        }

        public void SendWhisper(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
                return;

            SendMessage(new WhisperComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }

        public void SendChat(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
                return;

            SendMessage(new ChatComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }

        public void SendNotification(string Message)
        {
            SendMessage(new BroadcastMessageAlertComposer(Message));
        }

        public void SendMessage(IServerPacket Message)
        {
            byte[] bytes = Message.GetBytes();

            if (Message == null)
                return;

            if (GetConnection() == null)
                return;

            GetConnection().SendData(bytes);
        }

        public void SendShout(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
                return;

            SendMessage(new ShoutComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }

        public int ConnectionID
        {
            get { return _id; }
        }

        public ConnectionInformation GetConnection()
        {
            return _connection;
        }

        public Habbo GetHabbo()
        {
            return _habbo;
        }

        public void Disconnect()
        {
            try
            {
                if (GetHabbo() != null)
                {
                    using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery(GetHabbo().GetQueryString);
                    }

                    GetHabbo().OnDisconnect();


                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }


            if (!_disconnected)
            {
                if (_connection != null)
                _connection.Dispose();
                _disconnected = true;
            }
        }

        public void Dispose()
        {
            if (GetHabbo() != null)
                GetHabbo().OnDisconnect();

            this.MachineId = string.Empty;
            this._disconnected = true;
            this._habbo = null;
            this._connection = null;
            this.RC4Client = null;
            this._packetParser = null;
        }

        public class Meteorologia
        {
            public string ApiVersion { get; set; }
            public Data Data { get; set; }
        }

        public class Data
        {
            public string Location { get; set; }
            public string Temperature { get; set; }
            public string Skytext { get; set; }
            public string Humidity { get; set; }
            public string Wind { get; set; }
            public string Date { get; set; }
            public string Day { get; set; }
        }
    }
}
