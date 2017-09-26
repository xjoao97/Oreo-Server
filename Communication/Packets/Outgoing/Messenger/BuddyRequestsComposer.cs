﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.Users.Messenger;
using Quasar.HabboHotel.Cache;

namespace Quasar.Communication.Packets.Outgoing.Messenger
{
    class BuddyRequestsComposer : ServerPacket
    {
        public BuddyRequestsComposer(ICollection<MessengerRequest> Requests)
            : base(ServerPacketHeader.BuddyRequestsMessageComposer)
        {
            base.WriteInteger(Requests.Count);
            base.WriteInteger(Requests.Count);

            foreach (MessengerRequest Request in Requests)
            {
                base.WriteInteger(Request.From);
               base.WriteString(Request.Username);

                UserCache User = QuasarEnvironment.GetGame().GetCacheManager().GenerateUser(Request.From);
               base.WriteString(User != null ? User.Look : "");
            }
        }
    }
}
