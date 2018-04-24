using Oreo.Database.Interfaces;
using System.Collections.Generic;
using Oreo.Database.Adapter;

namespace Oreo.HabboHotel.Badges
{
    public static class BadgeDao
    {
        public static void LoadBadges(Dictionary<string, BadgeDefinition> badges)
        {
            badges.Clear();

            using (IQueryAdapter dbClient = OreoServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `badge_definitions`;");
                using (var reader = dbClient.ExecuteReader())
                    while (reader.Read())
                    {
                        string BadgeCode = reader.GetString("code").ToUpper();
                        badges.Add(BadgeCode, new BadgeDefinition(BadgeCode, reader.GetString("required_right")));
                    }
            }
        }
    }
}