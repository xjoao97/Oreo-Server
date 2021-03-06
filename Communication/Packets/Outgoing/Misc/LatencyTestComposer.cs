using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.Communication.Packets.Outgoing.Misc
{
    class LatencyTestComposer : ServerPacket
    {
        public LatencyTestComposer(int testResponce)
            : base(ServerPacketHeader.LatencyResponseMessageComposer)
        {
            base.WriteInteger(testResponce);
        }
    }
}
