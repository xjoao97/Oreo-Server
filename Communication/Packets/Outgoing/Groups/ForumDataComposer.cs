using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Outgoing.Groups
{
    class GetGroupForumsMessageEvent : ServerPacket
    {
        public GetGroupForumsMessageEvent(GroupForum Forum, GameClient Session)
            : base(ServerPacketHeader.GroupForumDataMessageComposer)
        {
            base.WriteInteger(Forum.Id);
            base.WriteString(Forum.Group.Name);
            base.WriteString(Forum.Group.Description);
            base.WriteString(Forum.Group.Badge);

            base.WriteInteger(Forum.Threads.Count);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteString("not_member");
            base.WriteInteger(0);

            base.WriteInteger(Forum.Settings.WhoCanRead);
            base.WriteInteger(Forum.Settings.WhoCanPost);
            base.WriteInteger(Forum.Settings.WhoCanInitDiscussions);
            base.WriteInteger(Forum.Settings.WhoCanModerate);

            base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanRead)); //Forum disabled reason//base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanRead));
            base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanPost));
            base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanInitDiscussions));
            base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate));
            base.WriteString("");

            base.WriteBoolean(Forum.Group.CreatorId == Session.GetHabbo().Id);
            base.WriteBoolean(Forum.Group.IsAdmin(Session.GetHabbo().Id) && Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate) == "");

        }
    }
}
