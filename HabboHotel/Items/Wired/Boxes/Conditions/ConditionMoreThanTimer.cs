using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Oreo.Communication.Packets.Incoming;
using Oreo.HabboHotel.Users;
using Oreo.HabboHotel.Rooms;
using Oreo.HabboHotel.Pathfinding;

namespace Oreo.HabboHotel.Items.Wired.Boxes.Conditions
{
    class MoreThanTimer : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.ConditionMoreThanTimer; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }
        public int timeout { get; set; }
        public string room { get; set; }

        public MoreThanTimer(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public int Time
        {
            get
            {
                return timeout;
            }
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Unknown2 = Packet.PopInt();

            this.StringData = Unknown2.ToString();
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            if (Instance == null)
                return false;

            return ((DateTime.Now - Instance.lastTimerReset).TotalSeconds < (timeout / 2));

        }



    }
}