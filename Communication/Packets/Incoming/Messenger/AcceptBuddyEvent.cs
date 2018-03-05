using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Users.Messenger;

namespace Quasar.Communication.Packets.Incoming.Messenger
{
    class AcceptBuddyEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            int Amount = Packet.PopInt();
            if (Amount > 50)
                Amount = 50;
            else if (Amount < 0)
                return;
                
                GameClient To = Oreoserver.GetGame().GetClientManager().GetClientByUserID(Request.To);
                GameClient From = Oreoserver.GetGame().GetClientManager().GetClientByUserID(Request.From);

                if (To != null && From != null)
                {
                    To.SendMessage(new RoomNotificationComposer("Friend Add", "Adicionado com sucesso  " + From.GetHabbo().Username + ".", "friendAdd");
                    From.SendMessage(new RoomNotificationComposer("Friend Add", "Adicionado com sucesso  " + To.GetHabbo().Username + ".", "friendAdd");
                }

            for (int i = 0; i < Amount; i++)
            {
                int RequestId = Packet.PopInt();

                MessengerRequest Request = null;
                if (!Session.GetHabbo().GetMessenger().TryGetRequest(RequestId, out Request))
                    continue;

                if (Request.To != Session.GetHabbo().Id)
                    return;

                if (!Session.GetHabbo().GetMessenger().FriendshipExists(Request.To))
                    Session.GetHabbo().GetMessenger().CreateFriendship(Request.From);

                Session.GetHabbo().GetMessenger().HandleRequest(RequestId);
            }
        }
    }
}
