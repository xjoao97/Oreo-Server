using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;

namespace Quasar.HabboHotel.Items.Wired.Boxes.Conditions
{
    class UserCountDoesntInRoomBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.ConditionUserCountDoesntInRoom; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public UserCountDoesntInRoomBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int CountOne = Packet.PopInt();
            int CountTwo = Packet.PopInt();

            this.StringData = CountOne + ";" + CountTwo;
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0)
                return false;

            if (String.IsNullOrEmpty(this.StringData))
                return false;

            int CountOne = this.StringData != null ? int.Parse(this.StringData.Split(';')[0]) : 1;
            int CountTwo = this.StringData != null ? int.Parse(this.StringData.Split(';')[1]) : 50;

            if (this.Instance.UserCount >= CountOne && this.Instance.UserCount <= CountTwo)
                return false;

            return true;
        }
    }
}