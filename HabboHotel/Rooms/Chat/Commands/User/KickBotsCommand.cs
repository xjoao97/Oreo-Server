using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms.AI;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;

using Quasar.HabboHotel.Users.Inventory.Bots;
using Quasar.Communication.Packets.Outgoing.Inventory.Bots;

using Quasar.Database.Interfaces;


namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class KickBotsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_kickbots"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Expulsar todos os BOTs dentro de sua sala."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, apenas o dono do quarto pode executar esse comando");
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.IsPet || !User.IsBot)
                    continue;

                RoomUser BotUser = null;
                if (!Room.GetRoomUserManager().TryGetBot(User.BotData.Id, out BotUser))
                    return;

                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `bots` SET `room_id` = '0' WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("id", User.BotData.Id);
                    dbClient.RunQuery();
                }

                Session.GetHabbo().GetInventoryComponent().TryAddBot(new Bot(Convert.ToInt32(BotUser.BotData.Id), Convert.ToInt32(BotUser.BotData.ownerID), BotUser.BotData.Name, BotUser.BotData.Motto, BotUser.BotData.Look, BotUser.BotData.Gender));
                Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
                Room.GetRoomUserManager().RemoveBot(BotUser.VirtualId, false);
            }

            Session.SendWhisper("Todos os bots foram removidos.");
        }
    }
}
