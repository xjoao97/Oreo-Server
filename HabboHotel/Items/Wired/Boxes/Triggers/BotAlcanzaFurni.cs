using Oreo.HabboHotel.Items;
using Oreo.HabboHotel.Rooms.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oreo.Communication.Packets.Incoming;
using Oreo.HabboHotel.Rooms;
using System.Collections.Concurrent;
using Oreo.HabboHotel.Users;
using Oreo.HabboHotel.Rooms.Games.Teams;

namespace Oreo.HabboHotel.Items.Wired.Boxes.Triggers
{
    class BotAlcanzaFurni : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.BotAlcanzaFurni; } }
        public string StringData { get; set; }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public bool BoolData { get; set; }
        public int Delay { get { return this._delay; } set { this._delay = value; this.TickCount = value; } }
        public int TickCount { get; set; }
        public string ItemsData { get; set; }
        private readonly UserWalksFurniDelegate delegateFunction;
        private String botname;
        private List<Room> items;

        private int _delay = 0;

        public List<Room> Items
        {
            get
            {
                return Items;
            }
        }

        public String Botname
        {
            get
            {
                return Botname;
            }
        }

        public bool Execute(params object[] Params)
        {
            return true;
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Delay = Packet.PopInt();

            this.Delay = Delay;
            this.TickCount = Delay;
        }

        public BotAlcanzaFurni(Room item, RoomUserManager roomUserManager, List<Room> items, string botname)
        {
            this.Item = Item;
            this.Instance = Instance;
            this.items = items;
            this.botname = botname;

            delegateFunction = roomUserManager_OnBotTakeItem;

            foreach (var targetItem in items)
            {
                targetItem.OnBotWalksOnFurni += delegateFunction;
            }
        }

        private void roomUserManager_OnBotTakeItem(object sender, EventArgs e)
        {
            var user = (RoomUser)sender;
            if (user != null && user.GetUsername().ToLower() == Botname.ToLower())
                Instance.GetRoomItemHandler();
        }

        public void Dispose()
        {
            Instance = null;
            botname = null;
            if (Items != null)
            {
                foreach (var targetItem in Items)
                {
                    targetItem.OnBotWalksOnFurni -= delegateFunction;
                }
                Items.Clear();
            }
            Item = null;
        }

       
        }
    }