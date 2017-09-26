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


namespace Quasar.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class SpecialEvent : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_addpredesigned";
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
                return "Manda un evento a todo el hotel.";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);

            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("¿Qué está pasando en " + QuasarEnvironment.GetDBConfig().DBData["hotel.name"] + "...?",
                 "Algo está ocurriendo en Habbi, Custom, HiddenKey y Root han desaparecido en medio de la ceremonia...<br><br>Un ente susurra y pide ayuda a todo Habbi, parece que los espíritus reclaman la presencia de todos nuestros usuarios.<br></font></b><br>Si quieres colaborar haz click en el botón inferior y sigue las instrucciones.<br><br></font>", "2mesex", "¡A la aventura!", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));

        }
    }
}

