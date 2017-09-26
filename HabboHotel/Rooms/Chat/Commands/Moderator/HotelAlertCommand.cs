﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class HotelAlertCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_hotel_alert"; }
        }

        public string Parameters
        {
            get { return "%message%"; }
        }

        public string Description
        {
            get { return "Envia un mensaje a todo el Hotel"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor escribe el mensaje a enviar");
                return;
            }
            string Message = CommandManager.MergeParams(Params, 1);
            if(QuasarEnvironment.GetDBConfig().DBData["hotel.name"] == "Habbi")
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Mensaje de " + Session.GetHabbo().Username + ":", "<font size =\"11\">Querido usuario de " + QuasarEnvironment.GetDBConfig().DBData["hotel.name"] + ", el usuario "+ Session.GetHabbo().Username +" tiene un mensaje para todo el hotel:</font><br><br><font size =\"11\" color=\"#B40404\">" + Message + "</font><br><br><font size =\"10\" color=\"#0B4C5F\">Recuerda estar atent@ a las redes sociales para mantenerte siempre al d\x00eda de las actualizaciones en Habbi Hotel:<br><br><b>FACEBOOK</b>: @EsHabbiHotel<br><b>TWITTER</b>: @EsHabbi<br><b>INSTAGRAM:</b> @EsHabbi</font>", "alertz", ""));
                  else
                QuasarEnvironment.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer(QuasarEnvironment.GetGame().GetLanguageLocale().TryGetValue("hotelalert_text") + Message + "\r\n- " + Session.GetHabbo().Username, ""));
            return;
        }
    }
}
