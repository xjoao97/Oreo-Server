﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Outgoing.Help.Helpers
{
    class CallForHelperErrorComposer : ServerPacket
    {
        public CallForHelperErrorComposer(int errorCode)
            : base(ServerPacketHeader.CallForHelperErrorMessageComposer)
        {
            base.WriteInteger(errorCode);
        }
    }
}
