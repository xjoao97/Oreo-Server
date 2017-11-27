using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class EnableFriends : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_enable_friends"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Habilitar ou desativar solicitações de amigos."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowFriendRequests = !Session.GetHabbo().AllowFriendRequests;
            Session.SendWhisper("Agora" + (Session.GetHabbo().AllowFriendRequests == true ? "você aceita" : "você não aceita") + " pedidos de amizade!");

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `block_newfriends` = '0' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.RunQuery();
            }
        }
    }
}
