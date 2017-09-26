﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Outgoing.Help.Helpers
{
    class EndHelperSessionComposer : ServerPacket
    {
        public EndHelperSessionComposer(int closeCode = 0)
            : base(ServerPacketHeader.EndHelperSessionMessageComposer)
        {
            base.WriteInteger(closeCode);
        }
    }
}
