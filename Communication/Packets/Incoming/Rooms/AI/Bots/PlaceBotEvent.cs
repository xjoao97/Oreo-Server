using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users.Inventory.Bots;
using Quasar.Communication.Packets.Outgoing.Inventory.Bots;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Rooms.AI.Speech;
using Quasar.HabboHotel.Rooms.AI;
using Quasar.HabboHotel.Rooms.AI.Responses;

namespace Quasar.Communication.Packets.Incoming.Rooms.AI.Bots
{
    class PlaceBotEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session, true))
                return;

            int BotId = Packet.PopInt();
            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if (!Room.GetGameMap().CanWalk(X, Y, false) || !Room.GetGameMap().ValidTile(X, Y))
            {
                Session.SendNotification("Você não pode colocar um bot aqui!");
                return;
            }

            Bot Bot = null;
            if (!Session.GetHabbo().GetInventoryComponent().TryGetBot(BotId, out Bot))
                return;

            int BotCount = 0;
            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList())
            {
                if (User == null || User.IsPet || !User.IsBot)
                    continue;

                BotCount += 1;
            }

            if (BotCount >= 5 && !Session.GetHabbo().GetPermissions().HasRight("bot_place_any_override"))
            {
                Session.SendNotification("Você só pode colocar 5 Bot's em uma sala!");
                return;
            }

            //TODO: Revisar
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `bots` SET `room_id` = '" + Room.RoomId + "', `x` = @CoordX, `y` = @CoordY WHERE `id` = @BotId LIMIT 1");
                dbClient.AddParameter("BotId", Bot.Id);
                dbClient.AddParameter("CoordX", X);
                dbClient.AddParameter("CoordY", Y);
                dbClient.RunQuery();
            }

            List<RandomSpeech> BotSpeechList = new List<RandomSpeech>();


            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `ai_type`,`rotation`,`walk_mode`,`automatic_chat`,`speaking_interval`,`mix_sentences`,`chat_bubble` FROM `bots` WHERE `id` = @BotId LIMIT 1");
                dbClient.AddParameter("BotId", Bot.Id);
                
				using (var reader = dbClient.ExecuteReader())
                   if (reader.Read())
                    {
                        RoomUser BotUser = Room.GetRoomUserManager().DeployBot(new RoomBot(Bot.Id, Session.GetHabbo().CurrentRoomId, reader.GetString("ai_type"), reader.GetString("walk_mode"), Bot.Name, "", Bot.Figure, X, Y, 0, 4, 0, 0, 0, 0, ref BotSpeechList, "", 0, Bot.OwnerId, PlusEnvironment.EnumToBool(reader.GetString("automatic_chat")), reader.GetInt32("speaking_interval"), PlusEnvironment.EnumToBool(reader.GetString("mix_sentences")), reader.GetInt32("chat_bubble")), null);
                        BotUser.Chat("Hello!", false, 0);

                        Room.GetGameMap().UpdateUserMovement(new System.Drawing.Point(X, Y), new System.Drawing.Point(X, Y), BotUser);
                    }

                dbClient.SetQuery("SELECT `text` FROM `bots_speech` WHERE `bot_id` = @BotId");
                dbClient.AddParameter("BotId", Bot.Id);
                using (var reader = dbClient.ExecuteReader())
                    while (reader.Read())
                    {
                        BotSpeechList.Add(new RandomSpeech(reader.GetString("text"), Bot.Id));
                    }
            }


            if (!Session.GetHabbo().GetInventoryComponent().TryRemoveBot(BotId, out Bot ToRemove))
            {
                Console.WriteLine("Erro ao recolher o Bot: " + ToRemove.Id);
                return;
            }
            Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
        }
    }
}
