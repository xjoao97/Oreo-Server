using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Navigator;
using Quasar.Communication.Packets.Outgoing.Rooms.Session;

namespace Quasar.Communication.Packets.Incoming.Rooms.Action
{
    class LetUserInEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room Room;

            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session))
                return;

            string Name = Packet.PopString();
            bool Accepted = Packet.PopBoolean();

            GameClient Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Name);
            if (Client == null)
                return;

            if (Accepted)
            {
                Client.GetHabbo().RoomAuthOk = true;
                Client.SendMessage(new FlatAccessibleComposer(""));
                Room.SendMessage(new FlatAccessibleComposer(Client.GetHabbo().Username), true);
            }
            else
            {
                Client.SendMessage(new FlatAccessDeniedComposer(""));
                Room.SendMessage(new FlatAccessDeniedComposer(Client.GetHabbo().Username), true);
            }
        }
    }
}
