﻿using System;
using System.Collections.Concurrent;

using Quasar.Communication.Packets.Incoming;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveUserBadgeBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.EffectGiveUserBadge; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public GiveUserBadgeBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Badge = Packet.PopString();

            this.StringData = Badge;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Owner = QuasarEnvironment.GetHabboById(Item.UserID);
            if (Owner == null || !Owner.GetPermissions().HasRight("room_item_wired_rewards"))
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null)
                return false;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return false;

            if (String.IsNullOrEmpty(StringData))
                return false;

            if (Player.GetBadgeComponent().HasBadge(StringData))
                Player.GetClient().SendMessage(new RoomCustomizedAlertComposer("Parece que você já tem esse Emblema, veja seu Inventário!"));
            else
            {
                Player.GetBadgeComponent().GiveBadge(StringData, true, Player.GetClient());
                Player.GetClient().SendMessage(new RoomCustomizedAlertComposer("${wiredfurni.badgereceived.body}"));
            }

            return true;
        }
    }
}
