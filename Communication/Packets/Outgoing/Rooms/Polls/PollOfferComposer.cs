using Quasar.HabboHotel.Rooms.Polls;

namespace Quasar.Communication.Packets.Outgoing.Rooms.Polls
{
    class PollOfferComposer : ServerPacket
    {
        public PollOfferComposer(RoomPoll poll)
            : base(ServerPacketHeader.PollOfferMessageComposer)
        {
            base.WriteInteger(poll.Id);
            base.WriteString(RoomPollTypeUtility.GetRoomPollType(poll.Type).ToUpper());
            base.WriteString(poll.Headline);
            base.WriteString(poll.Summary);
        }
    }
}