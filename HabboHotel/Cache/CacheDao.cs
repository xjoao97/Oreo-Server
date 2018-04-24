using Oreo.Database.Interfaces;
using Oreo.HabboHotel.Cache.Type;

namespace Oreo.HabboHotel.Cache
{
    public static class CacheDao
    {
        public static UserCache GenerateUser(int id)
        {
            using (IQueryAdapter dbClient = OreoServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username`, `motto`, `look` FROM users WHERE id = @id LIMIT 1");
                dbClient.AddParameter("id", id);

                using (var reader = dbClient.ExecuteReader())
                    if (reader.Read())
                    {
                        return new UserCache(id, reader.GetString("username"), reader.GetString("motto"), reader.GetString("look"));
                    }
            }
            return null;
        }
    }
}