using Quasar.HabboHotel.Groups;
using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Groups;
namespace Quasar.Communication.Packets.Incoming.Groups
{
    class UpdateGroupBadgeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            Group Group = null;
            if (!QuasarEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;
            if (Group.CreatorId != Session.GetHabbo().Id)
                return;
            int Count = Packet.PopInt();
            string Badge = "";
            for (int i = 0; i < Count; i++)
            {
                Badge += BadgePartUtility.WorkBadgeParts(i == 0, Packet.PopInt().ToString(), Packet.PopInt().ToString(), Packet.PopInt().ToString());
            }
            Group.Badge = (string.IsNullOrWhiteSpace(Badge) ? "b05114s06114" : Badge);
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET `badge` = @badge WHERE `id` = @groupId LIMIT 1");
                dbClient.AddParameter("badge", Group.Badge);
                dbClient.AddParameter("groupId", Group.Id);
                dbClient.RunQuery();
            }
            Session.SendMessage(new GroupInfoComposer(Group, Session));
        }
    }
}