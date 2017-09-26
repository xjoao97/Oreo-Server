﻿using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.GameClients;

using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class LastMessagesCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_user_info"; }
        }

        public string Parameters
        {
            get { return "%username%"; }
        }

        public string Description
        {
            get { return "Consulta los últimos mensajes del usuario."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Introduce el nombre del usuario que deseas ver revisar su información.");
                return;
            }

            DataRow UserData = null;
            string Username = Params[1];

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username` FROM users WHERE `username` = @Username LIMIT 1");
                dbClient.AddParameter("Username", Username);
                UserData = dbClient.getRow();
            }

            if (UserData == null)
            {
                Session.SendNotification("No existe ningún usuario con el nombre " + Username + ".");
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);

            DataTable GetLogs = null;
            StringBuilder HabboInfo = new StringBuilder();

            HabboInfo.Append("Estos son los últimos mensajes del usuario sospechoso, recuerda revisar siempre estos casos antes de proceder a banear a menos que sea un  caso evidente de spam.\n\n");

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `message` FROM `chatlogs` WHERE `user_id` = '" + TargetClient.GetHabbo().Id + "' ORDER BY `id` DESC LIMIT 10");
                GetLogs = dbClient.getTable();

                if (GetLogs != null)
                {
                    int Number = 11;
                    foreach (DataRow Log in GetLogs.Rows)
                    {
                        Number -= 1;
                        HabboInfo.Append("<font size ='8' color='#B40404'><b>[" + Number + "]</b></font>" + " " + Convert.ToString(Log["message"]) + "\r");
                    }
                }

                Session.SendMessage(new RoomNotificationComposer("Últimos mensajes de " + Username + ":", (HabboInfo.ToString()), "usr/body/" + Username + "", "", ""));
            

                //Session.SendMessage(new RoomCustomizedAlertComposer("Lamentablemente el usuario que has solicitado no tiene mensajes en el registro."));


            }
        }
    }
}
    
