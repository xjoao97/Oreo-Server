using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;

namespace Quasar.Communication.Packets.Incoming.Rooms.Furni.Stickys
{
    class UpdateStickyNoteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            Item Item = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Item == null || Item.GetBaseItem().InteractionType != InteractionType.POSTIT)
                return;

            String Color = Packet.PopString();
            String Text = Packet.PopString();

            if (!Room.CheckRights(Session))
            {
                if (!Text.StartsWith(Item.ExtraData))
                    return; // we can only ADD stuff! older stuff changed, this is not allowed
            }

            switch (Color)
            {
                case "FFFF33":
                case "FF9CFF":
                case "9CCEFF":
                case "9CFF9C":

                    break;

                default:

                    return;
            }

            Item.ExtraData = Color + " " + Text;
            Item.UpdateState(true, true);
        }
    }
}
