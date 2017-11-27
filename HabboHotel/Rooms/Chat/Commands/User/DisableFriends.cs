using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableFriends : IChatCommand
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
            get { return "Habilite ou desabilite as solicitações de amigos."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowFriendRequests = !Session.GetHabbo().AllowFriendRequests;
            Session.SendWhisper("Ahora mismo " + (Session.GetHabbo().AllowFriendRequests == true ? "não aceita" : "aceita") + " pedidos de amizade");

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `block_newfriends` = '0' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.RunQuery();
            }
        }
    }
}
