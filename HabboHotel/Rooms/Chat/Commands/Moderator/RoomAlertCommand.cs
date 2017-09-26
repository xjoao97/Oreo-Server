﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RoomAlertCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_room_alert"; }
        }

        public string Parameters
        {
            get { return "%message%"; }
        }

        public string Description
        {
            get { return "Enviar un mensaje a todos los usuarios en una sala."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor introduce el mensaje que deseas enviar en la sala");
                return;
            }

            if(!Session.GetHabbo().GetPermissions().HasRight("mod_alert") && Room.OwnerId != Session.GetHabbo().Id)
            {
                Session.SendWhisper("Solo puede hacerlo en su propia habitacion..");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (RoomUser == null || RoomUser.GetClient() == null || Session.GetHabbo().Id == RoomUser.UserId)
                    continue;

                RoomUser.GetClient().SendNotification(Message + "\n\n - " + Session.GetHabbo().Username);
            }
            Session.SendWhisper("Mensaje enviado correctamente en la sala.");
        }
    }
}
