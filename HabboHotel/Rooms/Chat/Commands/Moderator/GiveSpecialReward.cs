﻿using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Nux;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveSpecialReward : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_give_special"; }
        }

        public string Parameters
        {
            get { return "%username% %type% %amount%"; }
        }

        public string Description
        {
            get { return ""; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 0)
            {
                Session.SendWhisper("Por favor introduce un nombre de usuario para premiar.", 34);
                return;
            }

            GameClient Target = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Oops, No se ha conseguido este usuario!");
                return;
            }

            Target.SendMessage(QuasarEnvironment.GetGame().GetNuxUserGiftsManager().NuxUserGifts.Serialize());
            Session.SendWhisper("Has activado correctamente el premio especial para " + Target.GetHabbo().Username, 34);
        }
    }
}