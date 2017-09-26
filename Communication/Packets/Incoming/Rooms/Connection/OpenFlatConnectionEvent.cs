using System;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing;
using Quasar.Communication.Packets.Outgoing.Nux;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.Communication.Packets.Incoming.Rooms.Connection
{
    public class OpenFlatConnectionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int RoomId = Packet.PopInt();
            string Password = Packet.PopString();

            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk)
                Session.SendMessage(new RoomCustomizedAlertComposer("Você não se autenticou ainda!"));

            Session.GetHabbo().PrepareRoom(RoomId, Password);

        }
    }
}
