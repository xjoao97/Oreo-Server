using Oreo.Database.Interfaces;
using System.Collections.Generic;

namespace Oreo.HabboHotel.Games
{
    public static class GameDataDao
    {
        public static void LoadGameData(Dictionary<int, GameData> gameDatas)
        {
            gameDatas.Clear();

            using (IQueryAdapter dbClient = OreoServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`name`,`colour_one`,`colour_two`,`resource_path`,`string_three`,`game_swf`,`game_assets`,`game_server_host`,`game_server_port`,`socket_policy_port`,`game_enabled` FROM `games_config`");
                using (var reader = dbClient.ExecuteReader())
                    while (reader.Read())
                        gameDatas.Add(reader.GetInt32("id"), new GameData(reader.GetInt32("id"), reader.GetString("name"), reader.GetString("colour_one"), reader.GetString("colour_two"), reader.GetString("resource_path"), reader.GetString("string_three"), reader.GetString("game_swf"), reader.GetString("game_assets"), reader.GetString("game_server_host"), reader.GetString("game_server_port"), reader.GetString("socket_policy_port"), OreoServer.EnumToBool(reader.GetString("game_enabled"))));
            }
        }
    }
}