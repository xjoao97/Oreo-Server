using System;
using System.Collections.Concurrent;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Items.Wired.Boxes.Effects
{
    class ShowMessageBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.EffectShowMessage; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public string Message2 { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public ShowMessageBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Message = Packet.PopString();

            this.StringData = Message;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null || string.IsNullOrWhiteSpace(StringData))
                return false;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return false;

            string Message = StringData;
            string MessageFiltered = StringData;

            if (StringData.Contains("%user%")) MessageFiltered = MessageFiltered.Replace("%user%", Player.Username);
            if (StringData.Contains("%userid%")) MessageFiltered = MessageFiltered.Replace("%userid%", Convert.ToString(Player.Id));
            if (StringData.Contains("%honor%")) MessageFiltered = MessageFiltered.Replace("%honor%", Convert.ToString(Player.GOTWPoints));
            if (StringData.Contains("%pixeles%")) MessageFiltered = MessageFiltered.Replace("%pixeles%", Convert.ToString(Player.GOTWPoints));
            if (StringData.Contains("%duckets%")) MessageFiltered = MessageFiltered.Replace("%duckets%", Convert.ToString(Player.Duckets));
            if (StringData.Contains("%diamonds%")) MessageFiltered = MessageFiltered.Replace("%diamonds%", Convert.ToString(Player.Diamonds));
            if (StringData.Contains("%rank%")) MessageFiltered = MessageFiltered.Replace("%rank%", Convert.ToString(Player.Rank));

            if (StringData.Contains("%roomname%")) MessageFiltered = MessageFiltered.Replace("%roomname%", Player.CurrentRoom.Name);
            if (StringData.Contains("%roomusers%")) MessageFiltered = MessageFiltered.Replace("%userson%", Player.CurrentRoom.UserCount.ToString());
            if (StringData.Contains("%roomowner%")) MessageFiltered = MessageFiltered.Replace("%roomowner%", Player.CurrentRoom.OwnerName.ToString());
            if (StringData.Contains("%roomlikes%")) MessageFiltered = MessageFiltered.Replace("%roomlikes%", Player.CurrentRoom.Score.ToString());

            if (StringData.Contains("%userson%")) MessageFiltered = MessageFiltered.Replace("%userson%", QuasarEnvironment.GetGame().GetClientManager().Count.ToString());

            if (StringData.Contains("%SIT%"))
            {
                Message = Message.Replace("%SIT%", "Você se sentou");

                if (User.Statusses.ContainsKey("lie") || User.isLying || User.RidingHorse || User.IsWalking)
                    return false;

                if (!User.Statusses.ContainsKey("sit"))
                {
                    if ((User.RotBody % 2) == 0)
                    {
                        if (User == null)
                            return false;

                        try
                        {
                            User.Statusses.Add("sit", "1.0");
                            User.Z -= 0.35;
                            User.isSitting = true;
                            User.UpdateNeeded = true;
                        }
                        catch { }
                    }
                    else
                    {
                        User.RotBody--;
                        User.Statusses.Add("sit", "1.0");
                        User.Z -= 0.35;
                        User.isSitting = true;
                        User.UpdateNeeded = true;
                    }
                }
                else if (User.isSitting == true)
                {
                    User.Z += 0.35;
                    User.Statusses.Remove("sit");
                    User.Statusses.Remove("1.0");
                    User.isSitting = false;
                    User.UpdateNeeded = true;
                }
            }

            if (StringData.Contains("%STAND%"))
            {
                Message = Message.Replace("%STAND%", "Você levantou");
                if (User.isSitting)
                {
                    User.Statusses.Remove("sit");
                    User.Z += 0.35;
                    User.isSitting = false;
                    User.UpdateNeeded = true;
                }
                else if (User.isLying)
                {
                    User.Statusses.Remove("lay");
                    User.Z += 0.35;
                    User.isLying = false;
                    User.UpdateNeeded = true;
                }
            }

            if (StringData.Contains("%LAY%"))
            {
                Message = Message.Replace("%LAY%", "Você deitou");

                Room Room = Player.GetClient().GetHabbo().CurrentRoom;

                if (!Room.GetGameMap().ValidTile(User.X + 2, User.Y + 2) && !Room.GetGameMap().ValidTile(User.X + 1, User.Y + 1))
                {
                    Player.GetClient().SendWhisper("Oops, você não pode deitar aqui");
                    return false;
                }

                if (User.Statusses.ContainsKey("sit") || User.isSitting || User.RidingHorse || User.IsWalking)
                    return false;

                if (Player.GetClient().GetHabbo().Effects().CurrentEffect > 0)
                    Player.GetClient().GetHabbo().Effects().ApplyEffect(0);

                if (!User.Statusses.ContainsKey("lay"))
                {
                    if ((User.RotBody % 2) == 0)
                    {
                        if (User == null)
                            return false;

                        try
                        {
                            User.Statusses.Add("lay", "1.0 null");
                            User.Z -= 0.35;
                            User.isLying = true;
                            User.UpdateNeeded = true;
                        }
                        catch { }
                    }
                    else
                    {
                        User.RotBody--;//
                        User.Statusses.Add("lay", "1.0 null");
                        User.Z -= 0.35;
                        User.isLying = true;
                        User.UpdateNeeded = true;
                    }

                }
                else
                {
                    User.Z += 0.35;
                    User.Statusses.Remove("lay");
                    User.Statusses.Remove("1.0");
                    User.isLying = false;
                    User.UpdateNeeded = true;
                }
            }
            //}

            //if (!Player.GetClient().GetHabbo().GetPermissions().HasRight("word_filter_override") &&
            //    !QuasarEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out Message))
            //{
            //    Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, MessageFiltered, 0, 34));
            //}

            //            if (!Player.GetClient().GetHabbo().GetPermissions().HasRight("word_filter_override") &&
            //               QuasarEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out MessageFiltered))
            //{
            //    DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            //    dtDateTime = dtDateTime.AddSeconds(QuasarEnvironment.GetUnixTimestamp()).ToLocalTime();

            //    QuasarEnvironment.GetGame().GetClientManager().StaffAlert(new RoomNotificationComposer("Alerta de plublicidade Wired: ",
            //     "Atenção, um Wired está sendo usado em uma sala que emite uma mensagem:  <b> " + Message.ToUpper() + "</b> dentro da sala <b>" + User.GetClient().GetHabbo().CurrentRoom.Name + "</b>.\r\n", "", "Revisar", "event:navigator/goto/" + User.GetClient().GetHabbo().CurrentRoomId));

            //    Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Mensaje inapropiado.", 0, 34));
            //    return false;
            //}

            // else {
            Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, MessageFiltered, 0, 34));
                return true;

        }
    }
}
