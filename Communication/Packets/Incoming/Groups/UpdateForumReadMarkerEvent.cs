using Quasar.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.Groups
{
    class UpdateForumReadMarkerEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var length = Packet.PopInt();
            for (var i = 0; i < length; i++)
            {
                var forumid = Packet.PopInt();
                var postid = Packet.PopInt();
                var readall = Packet.PopBoolean();

                var forum = QuasarEnvironment.GetGame().GetGroupForumManager().GetForum(forumid);
                if (forum == null)
                    continue;

                var post = forum.GetPost(postid);
                if (post == null)
                    continue;

                var thread = post.ParentThread;
                var index = thread.Posts.IndexOf(post);
                thread.AddView(Session.GetHabbo().Id, index + 1);

            }
            //Thread.AddView(Session.GetHabbo().Id);
        }
    }
}
