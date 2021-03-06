using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.Users.Messenger;
using Quasar.HabboHotel.Users.Relationships;

namespace Quasar.Communication.Packets.Outgoing.Messenger
{
    class MessengerInitComposer : ServerPacket
    {
        public MessengerInitComposer()
            : base(ServerPacketHeader.MessengerInitMessageComposer)
        {
            base.WriteInteger(QuasarStaticGameSettings.MessengerFriendLimit);//Friends max.
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(1);
            base.WriteInteger(1);

            base.WriteInteger(1);
            base.WriteString("Chat de Grupos");
        }
    }
}
