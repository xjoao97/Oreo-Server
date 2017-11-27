using System.Collections.Concurrent;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Communication.Packets.Outgoing.Messenger;

namespace Quasar.HabboHotel.Items.Wired.Boxes.Effects
{
    class ProgressUserAchievementBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectProgressUserAchievement; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public ProgressUserAchievementBox(Room Instance, Item Item)
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


            Habbo Owner = QuasarEnvironment.GetHabboById(Item.UserID);
            if (Owner == null || Owner.Rank < 6)
            {
                this.StringData = "";
                Owner.GetClient().SendWhisper("Eu não sei quem lhe deu isso, mas você não estar usando..", 34);
                QuasarEnvironment.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, Owner.Username + " está usando um Wired da equipe!"));
            }
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

            var Message = StringData.Split('-');
            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_" + Message[0], int.Parse(Message[1]));
            return true;
        }
    }
}
