using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;



namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableMimicCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_disable_mimic"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ligue ou desligue a opção para copiar suas roupas."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowMimic = !Session.GetHabbo().AllowMimic;
            Session.SendWhisper("Você agora " + (Session.GetHabbo().AllowMimic == true ? "permite" : "não permite") + "que copiem sua roupa!");

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `allow_mimic` = @AllowMimic WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.AddParameter("AllowMimic", QuasarEnvironment.BoolToEnum(Session.GetHabbo().AllowMimic));
                dbClient.RunQuery();
            }
        }
    }
}
