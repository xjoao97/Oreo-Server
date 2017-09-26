﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Handshake;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class FlagUserCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_flaguser"; }
        }

        public string Parameters
        {
            get { return "%username%"; }
        }

        public string Description
        {
            get { return "Fuerza a algun usuario a cambiar su nombre."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, debe introducir el nombre del usuario al cual se le quiere cambiar el nombre");
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ha ocurrido un problema mientras se buscaba al usuario, o quizas no esta online");
                return;
            }

           else if (TargetClient.GetHabbo()._changename != 1)
            {
                Session.SendNotification("El usuario " +TargetClient.GetHabbo().Username+" no puede recibir el cambio de nombre, a causa de que ya ha agotado el cambio permitido.");
                TargetClient.SendNotification("¡Vaya!, uno de nuestros staffs ha intentado cambiarte el nombre, pero como lo has cambiado hace menos de un mes, no podemos proceder a tu cambio, si lo deseas, puedes comprar un cambio adicional dentro del catálogo");
                return;
            }


            else if (TargetClient.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Usted no puede elegir un nombre.");
                return;
            }
            else
            {
                TargetClient.GetHabbo().LastNameChange = 0;
                TargetClient.GetHabbo().ChangingName = true;
                TargetClient.SendNotification("Por favor se ha determinado que su nombre de usuario no es correcto o es inapropiado\r\rUn staff ha decidido darte una oportunidad para que puedas cambiar tu nombre, asi podrias evitar una expulsion del hotel.\r\rCierra esta ventana, y has clic sobre ti mismo y te saldra la opcion de cambiar nombre, Cambiatelo! \n\n <b><u>Recuerda que solo posees un cambio de nombre, piensa bien antes de elegirlo</b></u>");
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE users SET changename = '0' WHERE id = " + TargetClient.GetHabbo().Id + "");
                }
                TargetClient.GetHabbo()._changename = 0;
                TargetClient.SendMessage(new UserObjectComposer(TargetClient.GetHabbo()));
            }

        }
    }
}
