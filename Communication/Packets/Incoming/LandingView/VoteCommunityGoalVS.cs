using Quasar.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.LandingView
{
    class VoteCommunityGoalVS : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int VoteType = Packet.PopInt();

            if (VoteType == 1)
            {
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE landing_communitygoalvs SET left_votes = left_votes + 1 WHERE id = " + QuasarEnvironment.GetGame().GetCommunityGoalVS().GetId());
                }

                QuasarEnvironment.GetGame().GetCommunityGoalVS().IncreaseLeftVotes();
            }
            else if (VoteType == 2)
            {
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE landing_communitygoalvs SET right_votes = right_votes + 1 WHERE id = " + QuasarEnvironment.GetGame().GetCommunityGoalVS().GetId());
                }

                QuasarEnvironment.GetGame().GetCommunityGoalVS().IncreaseRightVotes();
            }
            QuasarEnvironment.GetGame().GetCommunityGoalVS().LoadCommunityGoalVS();
        }
    }
}
