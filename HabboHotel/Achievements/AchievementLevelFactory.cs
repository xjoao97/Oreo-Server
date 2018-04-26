using System;
using System.Collections.Generic;
using System.Data;
using Quasar.Database.Interfaces;

namespace Quasar.HabboHotel.Achievements
{
    public static class AchievementDao
    {
        public static void GetAchievementLevels(out Dictionary<string, Achievement> Achievements)
        {
            public static void LoadLevels(Dictionary<string, Achievement> achievements)

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`category`,`group_name`,`level`,`reward_pixels`,`reward_points`,`progress_needed`,`game_id` FROM `achievements`");
                
				using (var reader = dbClient.ExecuteReader())
                    while (reader.Read())
                    {
                        int id = reader.GetInt32("id");
                        string category = reader.GetString("category");
                        string groupName = reader.GetString("group_name");
                        int level = reader.GetInt32("level");
                        int rewardPixels = reader.GetInt32("reward_pixels");
                        int rewardPoints = reader.GetInt32("reward_points");
                        int progressNeeded = reader.GetInt32("progress_needed");

                        AchievementLevel AchievementLevel = new AchievementLevel(level, rewardPixels, rewardPoints, progressNeeded);

                        if (!achievements.ContainsKey(groupName))
                        {
                            Achievement Achievement = new Achievement(id, groupName, category, reader.GetInt32("game_id"));
                            Achievement.AddLevel(AchievementLevel);
                            achievements.Add(groupName, Achievement);
                        }
                        else
                            achievements[groupName].AddLevel(AchievementLevel);
                    }
                }
            }
        }
    }
}
