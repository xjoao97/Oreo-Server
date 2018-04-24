using Oreo.Database.Interfaces;
using Oreo.HabboHotel.Rooms.AI.Responses;
using System.Collections.Generic;

namespace Oreo.HabboHotel.Bots
{
    public static class BotDao
    {
        public static void LoadBotResponses(List<BotResponse> responses)
        {
            responses.Clear();

            using (IQueryAdapter dbClient = OreoServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `bot_ai`,`chat_keywords`,`response_text`,`response_mode`,`response_beverage` FROM `bots_responses`");
                using (var reader = dbClient.ExecuteReader())
                while (reader.Read())
                responses.Add(new BotResponse(reader.GetString("bot_ai"), reader.GetString("chat_keywords"), reader.GetString("response_text"), reader.GetString("response_mode"), reader.GetString("response_beverage")));
            }
        }
    }
}