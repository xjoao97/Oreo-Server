using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Items.RentableSpaces;

namespace Quasar.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces
{
    class BuyRentableSpaceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {

            int itemId = Packet.PopInt();

            Room room;
            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out room))
                return;

            if (room == null || room.GetRoomItemHandler() == null)
                return;

            RentableSpaceItem rsi;
            if (QuasarEnvironment.GetGame().GetRentableSpaceManager().GetRentableSpaceItem(itemId, out rsi))
            {
                QuasarEnvironment.GetGame().GetRentableSpaceManager().ConfirmBuy(Session, rsi, 3600);
            }

        }
    }
}
