/*using Quasar.HabboHotel.Users;

namespace Quasar.Communication.Packets.Outgoing.Avatar
{
    class onGuideSessionStarted : ServerPacket
    {
        public onGuideSessionStarted(Habbo user1, Habbo user2)
            : base(ServerPacketHeader.onGuideSessionStarted)
        {
            base.WriteInteger(user1.Id);
            base.WriteString(user1.Username);
            base.WriteString(user1.Look);
            base.WriteInteger(user2.Id);
            base.WriteString(user2.Username);
            base.WriteString(user2.Look);
        }
    }
}*/
