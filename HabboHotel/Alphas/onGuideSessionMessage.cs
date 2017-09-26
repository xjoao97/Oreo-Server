/*﻿using HabboEvents;
using Atlanta.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlanta.Protocol.Messages.outgoing
{
    class onGuideSessionMessage
    {
        internal static ServerMessage composer(uint id, string message)
        {
            var m = new ServerMessage(Outgoing.onGuideSessionMessage);
            m.WriteString(message);
            m.WriteInt(id);
            return m;
        }
    }
}*/
