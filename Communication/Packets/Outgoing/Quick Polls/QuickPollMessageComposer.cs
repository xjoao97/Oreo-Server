using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Outgoing.Rooms.Poll
{
    class QuickPollMessageComposer : ServerPacket
    {
        public QuickPollMessageComposer(String question)
            : base(ServerPacketHeader.QuickPollMessageComposer)
        {
            base.WriteString("");
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(1);
            base.WriteInteger(-1);
            base.WriteInteger(120);
            base.WriteInteger(3);
            base.WriteString(question);
        }
    }
}
