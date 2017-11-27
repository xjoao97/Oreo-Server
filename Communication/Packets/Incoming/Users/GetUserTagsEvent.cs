using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Users;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.Communication.Packets.Incoming.Users
{
    class GetUserTagsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int UserId = Packet.PopInt();
            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);

            Session.SendMessage(new UserTagsComposer(UserId, TargetClient));

            if (UserId == 2)
            {
                //Session.SendMessage(new MassEventComposer("habbopages/custom.txt"));
                //return;
            }
        }
    }
}
