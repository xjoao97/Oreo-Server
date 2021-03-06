using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Users.Messenger;
using Quasar.Communication.Packets.Outgoing.Messenger;

namespace Quasar.Communication.Packets.Incoming.Messenger
{
    class MessengerInitEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            Session.GetHabbo().GetMessenger().OnStatusChanged(false);

            ICollection<MessengerBuddy> Friends = new List<MessengerBuddy>();
            foreach (MessengerBuddy Buddy in Session.GetHabbo().GetMessenger().GetFriends())
            {
                if (Buddy == null || Buddy.IsOnline) continue;
                Friends.Add(Buddy);
            }

            Session.SendMessage(new MessengerInitComposer());
            Session.SendMessage(new BuddyListComposer(Friends, Session.GetHabbo()));

            try
            {
                if (Session.GetHabbo().GetMessenger() != null)
                    Session.GetHabbo().GetMessenger().OnStatusChanged(true);
            }
            catch { }

            Session.GetHabbo().GetMessenger().ProcessOfflineMessages();
        }
    }
}