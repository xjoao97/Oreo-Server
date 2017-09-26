using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Users;
using Quasar.Communication.Packets.Outgoing.Notifications;


using Quasar.Communication.Packets.Outgoing.Handshake;
using Quasar.Communication.Packets.Outgoing.Quests;
using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.HabboHotel.Quests;
using Quasar.HabboHotel.Rooms;
using System.Threading;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Avatar;
using Quasar.Communication.Packets.Outgoing.Pets;
using Quasar.Communication.Packets.Outgoing.Messenger;
using Quasar.HabboHotel.Users.Messenger; 
using Quasar.Communication.Packets.Outgoing.Rooms.Polls;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Communication.Packets.Outgoing.Availability;
using Quasar.Communication.Packets.Outgoing;
using Quasar.Communication.Packets.Outgoing.Help.Helpers;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class HelpCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_retired";
            }
        }
        public string Parameters
        {
            get { return "%message%"; }
        }
        public string Description
        {
            get
            {
                return "Envía una petición de ayuda, describiendo brevemente tu problema.";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            long nowTime = QuasarEnvironment.CurrentTimeMillis();
            long timeBetween = nowTime - Session.GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 60000)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("abuse","Espera al menos 1 minuto para volver a usar el sistema de soporte.",""));
                return;
            }

            Session.GetHabbo()._lastTimeUsedHelpCommand = nowTime;
            string Request = CommandManager.MergeParams(Params, 1);

            if (Params.Length == 1)
            {
                Session.SendMessage(new RoomNotificationComposer("Sistema de soporte:", "<font color='#B40404'><b>¡Atención, " + Session.GetHabbo().Username + "!</b></font>\n\n<font size=\"11\" color=\"#1C1C1C\">El sistema de soporte ha sido creado para hacer peticiones de ayuda detalladas. Por lo que no puedes enviar un mensaje vacío ya que no tiene ninguna utilidad.\n\n" +
                 "Si quieres pedir ayuda, describe <font color='#B40404'> <b> detalladamente tu problema</b></font>. \n\nEl sistema detectará si abusas de estas peticiones, por lo que no envíes más de una o serás bloqueado.\n\n" +
                 "Recuerda que también dispones de la central de ayuda para resolver tus problemas.", "quasarhelp", ""));
                return;
            }

            else 

            QuasarEnvironment.GetGame().GetClientManager().GuideAlert(new RoomNotificationComposer("¡Nuevo caso de atención!",
                 "El usuario " + Session.GetHabbo().Username + " requiere la ayuda de un guía, embajador o moderador.<br></font></b><br>Su duda o problema es el siguiente:<br>"
                 + Request + "</b></font><br><br>Atiende al usuario cuanto antes para resolver su duda, recuerda que dentro de poco tu ayuda será puntuada y eso se tendrá en cuenta para ascender.", "helpers", "Seguir a " + Session.GetHabbo().Username + "", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));

            //QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_GuideEnrollmentLifetime", 1);
            Session.SendMessage(new CallForHelperWindowComposer(false, 1, Request, 1));
            Session.SendMessage(RoomNotificationComposer.SendBubble("ambassador", "Tu petición de ayuda ha sido enviada correctamente, por favor espera.", ""));
        }
    }
}



