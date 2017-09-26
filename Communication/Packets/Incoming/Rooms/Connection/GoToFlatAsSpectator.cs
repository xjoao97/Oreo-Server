using System;
using System.Linq;
using System.Text;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Session;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.Communication.Packets.Outgoing.Nux;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;

namespace Quasar.Communication.Packets.Incoming.Rooms.Connection
{
    class GoToFlatAsSpectatorEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            /*Session.GetHabbo().Spectating = true;
            Session.SendMessage(new RoomSpectatorComposer());

            Room roomToSpec = Session.GetHabbo().CurrentRoom;

            roomToSpec.QueueingUsers.Remove(Session.GetHabbo());
            foreach (Habbo user in roomToSpec.QueueingUsers)
            {
                if (roomToSpec.QueueingUsers.First().Id == user.Id)
                {
                    user.PrepareRoom(roomToSpec.Id, "");
                }
                    else
                {
                    user.GetClient().SendMessage(new RoomQueueComposer(roomToSpec.QueueingUsers.IndexOf(user)));
                }
            }*/

            if (!Session.GetHabbo().EnterRoom(Session.GetHabbo().CurrentRoom))
                Session.SendMessage(new CloseConnectionComposer());
        }
    }
}
