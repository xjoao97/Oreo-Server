using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Talents;
using Quasar.Communication.Packets.Outgoing.Talents;

namespace Quasar.Communication.Packets.Incoming.Talents
{
    class GetTalentTrackEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string Type = Packet.PopString();

            ICollection<TalentTrackLevel> Levels = QuasarEnvironment.GetGame().GetTalentTrackManager().GetLevels();

            Session.SendMessage(new TalentTrackComposer(Levels, Type, Session));
        }
    }
}
