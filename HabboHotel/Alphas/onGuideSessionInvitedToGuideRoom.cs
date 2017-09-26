/*using HabboEvents;
using Atlanta.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlanta.Protocol.Messages.outgoing
{
    class onGuideSessionInvitedToGuideRoom
    {
        internal static ServerMessage composer(uint RoomId, string roomName)
        {
            var m = new ServerMessage(Outgoing.onGuideSessionInvitedToGuideRoom);
            m.WriteInt(RoomId);
            m.WriteString(roomName);
            return m;
        }
    }
}*/
