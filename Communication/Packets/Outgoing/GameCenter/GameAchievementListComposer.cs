using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Achievements;

namespace Quasar.Communication.Packets.Outgoing.GameCenter
{
    class GameAchievementListComposer : ServerPacket
    {
        public GameAchievementListComposer(GameClient Session, ICollection<Achievement> Achievements, int GameId)
            : base(ServerPacketHeader.GameAchievementListMessageComposer)
        {
            base.WriteInteger(GameId);
            base.WriteInteger(Achievements.Count);
            foreach (Achievement Ach in Achievements)
            {
                UserAchievement UserData = Session.GetHabbo().GetAchievementData(Ach.GroupName);
                int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);

                AchievementLevel TargetLevelData = Ach.Levels[TargetLevel];

                base.WriteInteger(Ach.Id);
                base.WriteInteger(TargetLevel);
                base.WriteString(Ach.GroupName + TargetLevel);
                base.WriteInteger(TargetLevelData.Requirement);
                base.WriteInteger(TargetLevelData.Requirement);
                base.WriteInteger(TargetLevelData.RewardPixels);
                base.WriteInteger(0);
                base.WriteInteger(UserData != null ? UserData.Progress : 0);
                base.WriteBoolean(UserData != null ? (UserData.Level >= Ach.Levels.Count) : false);
                base.WriteString(Ach.Category);
                base.WriteString("basejump");
                base.WriteInteger(0);
                base.WriteInteger(0);
            }
           base.WriteString("");
        }
    }
}
