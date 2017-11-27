using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;



namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableGiftsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_disable_gifts"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ativar ou desativar a recepção de presente."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowGifts = !Session.GetHabbo().AllowGifts;
            Session.SendWhisper("Você " + (Session.GetHabbo().AllowGifts == true ? "aceita" : "não aceita") + " presentes");

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `allow_gifts` = @AllowGifts WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.AddParameter("AllowGifts", QuasarEnvironment.BoolToEnum(Session.GetHabbo().AllowGifts));
                dbClient.RunQuery();
            }
        }
    }
}
