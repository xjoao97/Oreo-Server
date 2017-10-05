using System;
using System.Linq;
using System.Collections.Generic;

using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.Users.Messenger;
using Quasar.HabboHotel.Users.Relationships;

namespace Quasar.Communication.Packets.Outgoing.Messenger
{
    class BuddyListComposer : ServerPacket
    {
        public BuddyListComposer(ICollection<MessengerBuddy> Friends, Habbo Player)
            : base(ServerPacketHeader.BuddyListMessageComposer)
        {
            var friendCount = Friends.Count;
            if (Player._guidelevel >= 1) friendCount++;
            if (Player.Rank >= 5) friendCount++;

            base.WriteInteger(1);
            base.WriteInteger(0);
            var groups = QuasarEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Player.Id).Where(c => c.HasChat).ToList();
            base.WriteInteger(friendCount + groups.Count);

            foreach (var gp in groups.ToList())
            {
                base.WriteInteger(int.MinValue + gp.Id);
                base.WriteString(gp.Name);
                base.WriteInteger(1);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteString(gp.Badge);
                base.WriteInteger(1);
                base.WriteString(string.Empty);
                base.WriteShort(0);
                base.WriteString(string.Empty);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteBoolean(false);
                base.WriteShort(0);

                var group = new ServerPacket(ServerPacketHeader.FriendListUpdateMessageComposer);
                group.WriteInteger(1);
                group.WriteInteger(1);
                group.WriteString("Chat de Grupos");
                group.WriteInteger(1);
                group.WriteInteger(-1);
                group.WriteInteger(gp.Id);
                Player.GetClient().SendMessage(group);
            }

            foreach (MessengerBuddy Friend in Friends.ToList())
            {
                Relationship Relationship = Player.Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(Friend.UserId)).Value;

                base.WriteInteger(Friend.Id);
                base.WriteString(Friend.mUsername);
                base.WriteInteger(1);
                base.WriteBoolean(Friend.IsOnline);
                base.WriteBoolean(Friend.IsOnline && Friend.InRoom);
                base.WriteString(Friend.IsOnline ? Friend.mLook : string.Empty);
                base.WriteInteger(0);
                base.WriteString(Friend.IsOnline ? Friend.mMotto : string.Empty);
                base.WriteString(string.Empty);
                base.WriteString(string.Empty);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteBoolean(false);
                base.WriteShort(Relationship == null ? 0 : Relationship.Type);
            }

            #region Custom Chats
            if (Player.Rank >= 4)
            {
                base.WriteInteger(int.MinValue);
                base.WriteString("Equipe");
                base.WriteInteger(1);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteString("badgeADM");
                base.WriteInteger(0);
                base.WriteString(string.Empty);
                base.WriteString(string.Empty);
                base.WriteString(string.Empty);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteBoolean(false);
                base.WriteShort(0);
            }

            if (Player._guidelevel >= 1)
            {
                base.WriteInteger(int.MinValue + 1);
                base.WriteString("Embaixadores");
                base.WriteInteger(1);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteString("badgeAMB");
                base.WriteInteger(0);
                base.WriteString(string.Empty);
                base.WriteString(string.Empty);
                base.WriteString(string.Empty);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteBoolean(false);
                base.WriteShort(0);
            }
            #endregion

        }
    }
}