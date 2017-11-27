using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;


namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class MutePetsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_mute_pets"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "SilênciA tudo oque os animais de estimação dizem."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowPetSpeech = !Session.GetHabbo().AllowPetSpeech;
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `pets_muted` = '" + ((Session.GetHabbo().AllowPetSpeech) ? 1 : 0) + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            if (Session.GetHabbo().AllowPetSpeech)
                Session.SendWhisper("Mudança feita, agora você não pode ouvir o que os animais de estimação dizem.");
            else
                Session.SendWhisper("Mudança feita, agora você pode ouvir o que os animais de estimação dizem");
        }
    }
}
