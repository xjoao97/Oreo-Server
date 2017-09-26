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
    internal class CatalogUpdateAlert : IChatCommand
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
                return "Avisar de una actualización en el catálogo del hotel.";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("¡Actualización en el catálogo!",
              "¡El catálogo de <font color=\"#2E9AFE\"><b>" + QuasarEnvironment.GetDBConfig().DBData["hotel.name"]  + "</b></font> acaba de ser actualizado! Si quieres observar <b>las novedades</b> sólo debes hacer click en el botón de abajo.<br>", "cata", "Ir a la página", "event:catalog/open/" + Message));

            Session.SendWhisper("Catalogo actualizado satisfactoriamente.");
        }
    }
}

