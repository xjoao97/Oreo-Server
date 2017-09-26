﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.Communication.Packets.Outgoing.Catalog
{
    class PresentDeliverErrorMessageComposer : ServerPacket
    {
        public PresentDeliverErrorMessageComposer(bool CreditError, bool DucketError)
            : base(ServerPacketHeader.PresentDeliverErrorMessageComposer)
        {
            base.WriteBoolean(CreditError);
            base.WriteBoolean(DucketError);
        }
    }
}
