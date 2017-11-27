using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class StaffInfo : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_staffinfo"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Mostra os membros da equipe online."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Dictionary<Habbo, UInt32> clients = new Dictionary<Habbo, UInt32>();

            StringBuilder content = new StringBuilder();
            content.Append("Membros conectado no " + QuasarEnvironment.GetConfig().data["hotel.name"] + ":\r\n");

            foreach (var client in QuasarEnvironment.GetGame().GetClientManager()._clients.Values)
            {
                if (client != null && client.GetHabbo() != null && client.GetHabbo().Rank > 3)
                    clients.Add(client.GetHabbo(), (Convert.ToUInt16(client.GetHabbo().Rank)));
            }

            foreach (KeyValuePair<Habbo, UInt32> client in clients.OrderBy(key => key.Value))
            {
                if (client.Key == null)
                    continue;

                content.Append("" + client.Key.Username + "Cargo: " + client.Key.Rank + "] - está no quarto: " + ((client.Key.CurrentRoom == null) ? "Em nenhum quarto." : client.Key.CurrentRoom.RoomData.Name) + "\r\n");
            }

            Session.SendMessage(new MOTDNotificationComposer(content.ToString()));

            return;
        }
    }
}
