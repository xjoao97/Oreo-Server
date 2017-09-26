using Quasar.Communication.Packets.Outgoing;
using Quasar.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Outgoing.Users
{
    class ClubGiftRecievedComposer : ServerPacket
    {
        public ClubGiftRecievedComposer(GameClient Session) : base(ServerPacketHeader.ClubGiftRecievedComposer)
        {
            base.WriteString("PENE");
            base.WriteInteger(1);
            base.WriteString("b");
            base.WriteString("ADMIN");
        }
    }
}
