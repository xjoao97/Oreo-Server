using System;

using Quasar.HabboHotel.Users;

namespace Quasar.Communication.Packets.Outgoing.Handshake
{
    public class UserObjectComposer : ServerPacket
    {
        public UserObjectComposer(Habbo Habbo)
            : base(ServerPacketHeader.UserObjectMessageComposer)
        {
            base.WriteInteger(Habbo.Id);
            base.WriteString(Habbo.Username);
            base.WriteString(Habbo.Look);
            base.WriteString(Habbo.Gender.ToUpper());
            base.WriteString(Habbo.Motto);
            base.WriteString("");
            base.WriteBoolean(false);
            base.WriteInteger(Habbo.GetStats().Respect);
            base.WriteInteger(Habbo.GetStats().DailyRespectPoints);
            base.WriteInteger(Habbo.GetStats().DailyPetRespectPoints);
            base.WriteBoolean(false);
            base.WriteString(Habbo.LastOnline.ToString());
            base.WriteBoolean(Habbo.ChangingName);
            base.WriteBoolean(false);
        }
    }
}
