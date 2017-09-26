/*using HabboEvents;
using Atlanta.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlanta.Protocol.Messages.outgoing

{
    class onGuideSessionAttached
    {
        internal static ServerMessage composer(bool isGuide, string message)
        {
            var m = new ServerMessage(Outgoing.onGuideSessionAttached);
            m.WriteBool(isGuide);
            m.WriteInt(isGuide ? 0 : 1);
            m.WriteString(message);
            m.WriteInt(20);
            return m;
        }
    }
}*/
