﻿//using System;
//using System.Linq;
//using System.Text;
//using System.Collections.Generic;

//using Quasar.HabboHotel.Users;
//using Quasar.HabboHotel.Users.Messenger;
//using Quasar.HabboHotel.Users.Relationships;

//namespace Quasar.Communication.Packets.Outgoing.Messenger
//{
//    class BuddyListComposer : ServerPacket
//    {
//        public BuddyListComposer(ICollection<MessengerBuddy> Friends, Habbo Player)
//            : base(ServerPacketHeader.BuddyListMessageComposer)
//        {
//            base.WriteInteger(0);
//            base.WriteInteger(0);

//            var groups = QuasarEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Player.Id).Where(c => c.HasChat).ToList();

//            base.WriteInteger(Friends.Count + groups.Count);

//            foreach (var gp in groups)
//            {
//                base.WriteInteger(-gp.Id);
//                base.WriteString(gp.Name);
//                base.WriteInteger(0);//Gender.
//                base.WriteBoolean(true);
//                base.WriteBoolean(false);
//                base.WriteString(gp.Badge);
//                base.WriteInteger(1); // category id
//                base.WriteString("");
//                base.WriteString(string.Empty);//Alternative name?
//                base.WriteString(string.Empty);
//                base.WriteBoolean(true);
//                base.WriteBoolean(false);
//                base.WriteBoolean(false);//Pocket Habbo user.
//                base.WriteShort(0);
//            }

//            foreach (MessengerBuddy Friend in Friends.ToList())
//            {
//                Relationship Relationship = Player.Relationships.FirstOrDefault(x => x.Value.UserId == Convert.ToInt32(Friend.UserId)).Value;

//                base.WriteInteger(Friend.Id);
//                base.WriteString(Friend.mUsername);
//                base.WriteInteger(1);//Gender.
//                base.WriteBoolean(Friend.IsOnline);
//                base.WriteBoolean(Friend.IsOnline && Friend.InRoom);
//                base.WriteString(Friend.IsOnline ? Friend.mLook : string.Empty);
//                base.WriteInteger(0); // category id
//                base.WriteString(Friend.IsOnline ? Friend.mMotto : string.Empty);
//                base.WriteString(string.Empty);//Alternative name?
//                base.WriteString(string.Empty);
//                base.WriteBoolean(true);
//                base.WriteBoolean(false);
//                base.WriteBoolean(false);//Pocket Habbo user.
//                base.WriteShort(Relationship == null ? 0 : Relationship.Type);
//            }

//            #region Custom Chats
//            if (Player.Rank >= 5)
//            {
//                base.WriteInteger(int.MinValue);  // Int.MaxValue
//                base.WriteString("Administración");
//                base.WriteInteger(1);
//                base.WriteBoolean(true);
//                base.WriteBoolean(false);
//                base.WriteString("staff_admin");
//                base.WriteInteger(0);
//                base.WriteString(string.Empty);
//                base.WriteString(string.Empty);
//                base.WriteString(string.Empty);
//                base.WriteBoolean(true);
//                base.WriteBoolean(false);
//                base.WriteBoolean(false);
//                base.WriteShort(0);
//            }

//            if (Player._guidelevel >= 1)
//            {
//                base.WriteInteger(int.MinValue + 1);
//                base.WriteString("Guías");
//                base.WriteInteger(1);
//                base.WriteBoolean(true);
//                base.WriteBoolean(false);
//                base.WriteString("guias");
//                base.WriteInteger(0);
//                base.WriteString(string.Empty);
//                base.WriteString(string.Empty);
//                base.WriteString(string.Empty);
//                base.WriteBoolean(true);
//                base.WriteBoolean(false);
//                base.WriteBoolean(false);
//                base.WriteShort(0);
//            }
//            #endregion
//        }
//    }
//}

using System;
using System.Linq;
using System.Text;
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
                base.WriteString("badgeADMIN");
                base.WriteInteger(1);
                base.WriteString(string.Empty);
                base.WriteShort(0);//Alterar caso queira descrição
                base.WriteString(string.Empty);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteBoolean(false);
                base.WriteShort(0);
            }

            if (Player._guidelevel >= 1)
            {
                base.WriteInteger(int.MinValue + 1);
                base.WriteString("Guías");
                base.WriteInteger(1);
                base.WriteBoolean(true);
                base.WriteBoolean(false);
                base.WriteString("badgeGUIAS");
                base.WriteInteger(1);
                base.WriteString(string.Empty);
                base.WriteShort(0);//Alterar caso queira descrição
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
