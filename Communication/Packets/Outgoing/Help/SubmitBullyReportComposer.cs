﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.Communication.Packets.Outgoing.Help
{
    class SubmitBullyReportComposer : ServerPacket
    {
        public SubmitBullyReportComposer(int Result)
            : base(ServerPacketHeader.SubmitBullyReportMessageComposer)
        {
            base.WriteInteger(Result);
        }
    }
}
