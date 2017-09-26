﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Pathfinding;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class PushCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_push"; }
        }

        public string Parameters
        {
            get { return "%target%"; }
        }

        public string Description
        {
            get { return "Empujar a alguien desde tu posición."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Introduce el nombre del usuario que deseas empujar");
                return;
            }

            //if (!Room.PushEnabled && !Session.GetHabbo().GetPermissions().HasRight("room_override_custom_config"))
            //{
            //    Session.SendWhisper("Oops, al parecer el dueño de la sala ha deshabilitado la capacidad de usar el comando Push");
            //    return;
            //}

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocurrio un problema, al parecer el usuario no se encuentra online o usted no escribio bien el nombre");
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Ocurrio un error, escribe correctamente el nombre, el usuario NO se encuentra online o en la sala.");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Vamos! no querras empujarte a ti mismo");
                return;
            }

            if (TargetUser.TeleportEnabled)
            {
                Session.SendWhisper("Oops, you cannot push a user whilst they have their teleport mode enabled.");
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (!((Math.Abs(TargetUser.X - ThisUser.X) >= 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) >= 2)))
            {
                if (TargetUser.SetX - 1 == Room.GetGameMap().Model.DoorX)
                {
                    Session.SendWhisper("Por favor no lo empujes hacia la salida :(!");
                    return;
                }

                if (TargetUser.RotBody == 4)
                {
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 1);
                }

                if (ThisUser.RotBody == 0)
                {
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 1);
                }

                if (ThisUser.RotBody == 6)
                {
                    TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y);
                }

                if (ThisUser.RotBody == 2)
                {
                    TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y);
                }

                if (ThisUser.RotBody == 3)
                {
                    TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 1);
                }

                if (ThisUser.RotBody == 1)
                {
                    TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 1);
                }

                if (ThisUser.RotBody == 7)
                {
                    TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 1);
                }

                if (ThisUser.RotBody == 5)
                {
                    TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 1);
                }

                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*He empujado a " + Params[1] + "*", 0, ThisUser.LastBubble));
            }
            else
            {
                Session.SendWhisper("Oops, " + Params[1] + " no esta lo suficientemente cerca!");
            }
        }
    }
}
