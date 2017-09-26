﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveBadgeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_give_badge"; }
        }

        public string Parameters
        {
            get { return "%username% %badge%"; }
        }

        public string Description
        {
            get { return "Dar una placa a un usuario"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length != 3)
            {
                Session.SendWhisper("Introduce el nombre del usuario a quien deseas enviar una placa!");
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient != null)
            {
                if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge(Params[2]))
                {
                    TargetClient.GetHabbo().GetBadgeComponent().GiveBadge(Params[2], true, TargetClient);
                    if (TargetClient.GetHabbo().Id != Session.GetHabbo().Id)
                        TargetClient.SendNotification("You have just been given a badge!");
                    else
                        Session.SendWhisper("Ha enviado correctamente la placa  " + Params[2] + "!");
                }
                else
                    Session.SendWhisper("Oops, este usuario ya tiene la placa  (" + Params[2] + ") !");
                return;
            }
            else
            {
                Session.SendWhisper("Oops, no se ha encontrado al usuario!");
                return;
            }
        }
    }
}
