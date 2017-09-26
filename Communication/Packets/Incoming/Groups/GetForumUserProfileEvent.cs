using System.Collections.Generic;

using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.Groups;
using Quasar.HabboHotel.GameClients;

using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Users;

namespace Quasar.Communication.Packets.Incoming.Groups.Forums
{
    class GetForumUserProfileEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string username = Packet.PopString();

            Habbo targetData = QuasarEnvironment.GetHabboByUsername(username);
            if (targetData == null)
            {
                Session.SendNotification("Ocorreu um erro ao buscar o usuário!");
                return;
            }

            List<Group> groups = QuasarEnvironment.GetGame().GetGroupManager().GetGroupsForUser(targetData.Id);

            int friendCount = 0;
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `messenger_friendships` WHERE (`user_one_id` = @userid OR `user_two_id` = @userid)");
                dbClient.AddParameter("userid", targetData.Id);
                friendCount = dbClient.getInteger();
            }

            Session.SendMessage(new ProfileInformationComposer(targetData, Session, groups, friendCount));
        }
    }
}
