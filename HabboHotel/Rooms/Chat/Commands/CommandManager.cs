using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Database.Interfaces;

using Quasar.HabboHotel.Items.Wired;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Rooms.Chat.Commands.User;
using Quasar.HabboHotel.Rooms.Chat.Commands.User.Fun;
using Quasar.HabboHotel.Rooms.Chat.Commands.Moderator;
using Quasar.HabboHotel.Rooms.Chat.Commands.Moderator.Fun;
using Quasar.HabboHotel.Rooms.Chat.Commands.Administrator;
using Quasar.HabboHotel.Rooms.Chat.Commands.Events;

using Quasar.HabboHotel.Rooms.Chat.Commands.User.Fan;

using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Communication.Packets.Outgoing.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands
{
    public class CommandManager
    {
        /// <summary>
        /// Command Prefix only applies to custom commands.
        /// </summary>
        private string _prefix = ":";

        /// <summary>
        /// Commands registered for use.
        /// </summary>
        private readonly Dictionary<string, IChatCommand> _commands;

        /// <summary>
        /// The default initializer for the CommandManager
        /// </summary>
        public CommandManager(string Prefix)
        {
            this._prefix = Prefix;
            this._commands = new Dictionary<string, IChatCommand>();

            this.RegisterVIP();
            this.RegisterUser();
            this.RegisterEvents();
            this.RegisterModerator();
            this.RegisterAdministrator();
        }

        /// <summary>
        /// Request the text to parse and check for commands that need to be executed.
        /// </summary>
        /// <param name="Session">Session calling this method.</param>
        /// <param name="Message">The message to parse.</param>
        /// <returns>True if parsed or false if not.</returns>
        public bool Parse(GameClient Session, string Message)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentRoom == null || QuasarStaticGameSettings.IsGoingToBeClose)
                return false;

            if (!Message.StartsWith(_prefix))
                return false;

            Room room = Session.GetHabbo().CurrentRoom;

            if (room.GetFilter().CheckCommandFilter(Message))
                return false;

            if (Message == _prefix + "commandsstafflist")
            {
                StringBuilder List = new StringBuilder();
                List.Append("Aqui estão seus comandos disponíveis:\n\n");
                foreach (var CmdList in _commands.ToList())
                {
                    if (!string.IsNullOrEmpty(CmdList.Value.PermissionRequired))
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand(CmdList.Value.PermissionRequired))
                            continue;
                    }

                    List.Append(":" + CmdList.Key + " " + CmdList.Value.Parameters + "\n" + CmdList.Value.Description + ".\n\n");
                }
                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                return true;
            }

            Message = Message.Substring(1);
            string[] Split = Message.Split(' ');

            if (Split.Length == 0)
                return false;

            IChatCommand Cmd = null;
            if (_commands.TryGetValue(Split[0].ToLower(), out Cmd))
            {
                if (Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                    this.LogCommand(Session.GetHabbo().Id, Message, Session.GetHabbo().MachineId, Session.GetHabbo().Username);

                if (!string.IsNullOrEmpty(Cmd.PermissionRequired))
                {
                    if (!Session.GetHabbo().GetPermissions().HasCommand(Cmd.PermissionRequired))
                        return false;
                }

                Session.GetHabbo().IChatCommand = Cmd;
                Session.GetHabbo().CurrentRoom.GetWired().TriggerEvent(WiredBoxType.TriggerUserSaysCommand, Session.GetHabbo(), this);

                Cmd.Execute(Session, Session.GetHabbo().CurrentRoom, Split);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Registers the VIP set of commands.
        /// </summary>
        private void RegisterVIP()
        {
            this.Register("spull", new SuperPullCommand());
        }

        /// <summary>
        /// Registers the Events set of commands.
        /// </summary>
        private void RegisterEvents()
        {
            this.Register("eha", new EventAlertCommand());
        }

        /// <summary>
        /// Registers the default set of commands.
        /// </summary>
        private void RegisterUser()
        {
            this.Register("commands", new UserCommandsCommand()); //wulles
            this.Register("comandos", new UserCommandsCommand()); //wulles
            this.Register("habnam", new HabnamCommand()); //wulles
            this.Register("yyxxabxa", new YYXXABXACommand()); //wulles
            this.Register("moonwalk", new MoonwalkCommand()); //wulles
            this.Register("emoji", new EmojiCommand());
            this.Register("bola", new BallCommand());
            this.Register("item", new CarryCommand());
            this.Register("setbet", new SetBetCommand());
            this.Register("bubblebot", new BubbleBotCommand());
            this.Register("grupo", new GroepChatCommand());
            this.Register("spam", new DisableSpamCommand());
            this.Register("about", new AboutCommand());
            this.Register("vipstatus", new ViewVIPStatusCommand());
            this.Register("color", new ColourCommand());
            this.Register("pickall", new PickAllCommand());
            this.Register("ejectall", new EjectAllCommand());
            this.Register("lay", new LayCommand());
            this.Register("sit", new SitCommand());
            this.Register("prefix", new PrefixCommand());
            this.Register("stand", new StandCommand());
            this.Register("mutepets", new MutePetsCommand());
            this.Register("mutebots", new MuteBotsCommand());
            this.Register("mimic", new MimicCommand());
            this.Register("dance", new DanceCommand());
            this.Register("push", new PushCommand());
            this.Register("pull", new PullCommand());
            this.Register("enable", new EnableCommand());
            this.Register("seguir", new FollowCommand());
            this.Register("faceless", new FacelessCommand());
            this.Register("variables", new WiredVariable());
            this.Register("unload", new UnloadCommand());
            this.Register("reload", new Reloadcommand());
            this.Register("regenmaps", new RegenMaps());
            this.Register("empty", new EmptyItems());
            this.Register("setmax", new SetMaxCommand());
            this.Register("setspeed", new SetSpeedCommand());
            this.Register("diagonal", new DisableDiagonalCommand());
            this.Register("flagme", new FlagMeCommand());
            this.Register("kickpets", new KickPetsCommand());
            this.Register("kickbots", new KickBotsCommand());
            this.Register("room", new RoomCommand());
            this.Register("dnd", new DNDCommand());
            this.Register("disablegifts", new DisableGiftsCommand());
            this.Register("convertcredits", new ConvertCreditsCommand());
            this.Register("disablewhispers", new DisableWhispersCommand());
            this.Register("disablemimic", new DisableMimicCommand()); ;
            this.Register("spush", new SuperPushCommand());
            this.Register("superpush", new SuperPushCommand());
            this.Register("disablefriends", new DisableFriends());
            this.Register("enablefriends", new EnableFriends());
        }

        private void Register(string v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers the moderator set of commands.
        /// </summary>
        private void RegisterModerator()
        {
            this.Register("ban", new BanCommand());
            this.Register("mip", new MIPCommand());
            this.Register("ipban", new IPBanCommand());
            this.Register("userinfo", new UserInfoCommand());
            this.Register("roomunmute", new RoomUnmuteCommand());
            this.Register("roommute", new RoomMuteCommand());
            this.Register("roombadge", new RoomBadgeCommand());
            this.Register("roomalert", new RoomAlertCommand());
            this.Register("roomkick", new RoomKickCommand());
            this.Register("usermute", new MuteCommand());
            this.Register("unmute", new UnmuteCommand());
            this.Register("massbadge", new MassBadgeCommand());
            this.Register("kick", new KickCommand());
            this.Register("ha", new HotelAlertCommand());
            this.Register("notf", new SendNotificationAlert());
            this.Register("hal", new HALCommand());
            this.Register("give", new GiveCommand());
            this.Register("arfeu", new LandingCommand());
            this.Register("massgive", new MassGiveCommand());
            this.Register("roomgive", new RoomGiveCommand());
            this.Register("givebadge", new GiveBadgeCommand());
            this.Register("dc", new DisconnectCommand());
            this.Register("alert", new AlertCommand());
            this.Register("tradeban", new TradeBanCommand());
            this.Register("masspoll", new MassPollCommand());
            this.Register("poll", new PollCommand());
            this.Register("lastmsg", new LastMessagesCommand());
            this.Register("lastconsolemsg", new LastConsoleMessagesCommand());
            this.Register("teleport", new TeleportCommand());
            this.Register("summon", new SummonCommand());
            this.Register("override", new OverrideCommand());
            this.Register("massenable", new MassEnableCommand());
            this.Register("massdance", new MassDanceCommand());
            this.Register("freeze", new FreezeCommand());
            this.Register("unfreeze", new UnFreezeCommand());
            this.Register("fastwalk", new FastwalkCommand());
            this.Register("superfastwalk", new SuperFastwalkCommand());
            this.Register("coords", new CoordsCommand());
            this.Register("alleyesonme", new AllEyesOnMeCommand());
            this.Register("allaroundme", new AllAroundMeCommand());
            this.Register("forcesit", new ForceSitCommand());
            this.Register("ignorewhispers", new IgnoreWhispersCommand());
            this.Register("forcedeffects", new DisableForcedFXCommand());
            this.Register("makesay", new MakeSayCommand());
            this.Register("flaguser", new FlagUserCommand());
            this.Register("filter", new FilterCommand());

            //this.Register("quizz", new IdolQuizCommand());
        }

        /// <summary>
        /// Registers the administrator set of commands.
        /// </summary>
        private void RegisterAdministrator()
        {
            this.Register("developer", new DevelopperCommand());
            this.Register("ia", new SendGraphicAlertCommand());
            this.Register("iau", new SendImageToUserCommand());
            this.Register("viewevents", new ViewStaffEventListCommand());;
            this.Register("givespecial", new GiveSpecialReward());
            this.Register("bubble", new BubbleCommand());
            this.Register("update", new UpdateCommand());
            this.Register("deletegroup", new DeleteGroupCommand());
            this.Register("goto", new GOTOCommand());
            this.Register("summonall", new SummonAll());
            this.Register("viewinventary", new ViewInventaryCommand());
            this.Register("makevip", new MakeVipCommand());
            this.Register("removebadge", new RemoveBadgeCommand());
            this.Register("staffinfo", new StaffInfo());

            //this.Register("bubblealert", new TrollAlert()); - wulles can finish this after.
        }

        /// <summary>
        /// Registers a Chat Command.
        /// </summary>
        /// <param name="CommandText">Text to type for this command.</param>
        /// <param name="Command">The command to execute.</param>
        public void Register(string CommandText, IChatCommand Command)
        {
            this._commands.Add(CommandText, Command);
        }

        public static string MergeParams(string[] Params, int Start)
        {
            var Merged = new StringBuilder();
            for (int i = Start; i < Params.Length; i++)
            {
                if (i > Start)
                    Merged.Append(" ");
                Merged.Append(Params[i]);
            }

            return Merged.ToString();
        }

        public void LogCommand(int UserId, string Data, string MachineId, string Username)
        {
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `logs_client_staff` (`user_id`,`data_string`,`machine_id`,`timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                dbClient.AddParameter("UserId", UserId);
                dbClient.AddParameter("Data", Data);
                dbClient.AddParameter("MachineId", MachineId);
                dbClient.AddParameter("Timestamp", QuasarEnvironment.GetUnixTimestamp());
                dbClient.RunQuery();
            }

            if (Data == "regenmaps" || Data.StartsWith("c") || Data == "sa" || Data == "ga")
            { return; }

            else
                QuasarEnvironment.GetGame().GetClientManager().ManagerAlert(RoomNotificationComposer.SendBubble("generic", "" + Username + "\nUsou o comando:\n:" + Data + ".", ""));
        }

        public bool TryGetCommand(string Command, out IChatCommand IChatCommand)
        {
            return this._commands.TryGetValue(Command, out IChatCommand);
        }
    }
}
