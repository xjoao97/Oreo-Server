using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;

namespace Quasar.Communication.Packets.Incoming.Rooms.Engine
{
    class MoveWallItemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = null;
            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session))
                return;

            int itemID = Packet.PopInt();
            string wallPositionData = Packet.PopString();

            Item Item = Room.GetRoomItemHandler().GetItem(itemID);

            if (Item == null)
                return;

            try
            {
                string WallPos = Room.GetRoomItemHandler().WallPositionCheck(":" + wallPositionData.Split(':')[1]);
                Item.wallCoord = WallPos;
            }
            catch { return; }

            Room.GetRoomItemHandler().UpdateItem(Item);
            Room.SendMessage(new ItemUpdateComposer(Item, Room.OwnerId));
        }
    }
}
