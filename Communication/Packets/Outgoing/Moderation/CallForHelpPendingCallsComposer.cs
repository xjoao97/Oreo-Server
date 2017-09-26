using Quasar.HabboHotel.Moderation;
using Quasar.Utilities;

namespace Quasar.Communication.Packets.Outgoing.Moderation
{
    class CallForHelpPendingCallsComposer : ServerPacket
    {
        public CallForHelpPendingCallsComposer(ModerationTicket ticket)
            : base(ServerPacketHeader.CallForHelpPendingCallsMessageComposer)
        {
            base.WriteInteger(1);
            {
                base.WriteString(ticket.Id.ToString());
                base.WriteString(UnixTimestamp.FromUnixTimestamp(ticket.Timestamp).ToShortTimeString());
                base.WriteString(ticket.Issue);
            }
        }
    }
}
