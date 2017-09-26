﻿using System.Linq;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class MassBadgeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_mass_badge"; }
        }

        public string Parameters
        {
            get { return "%badge%"; }
        }

        public string Description
        {
            get { return "Envia una placa a todos los del hotel"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor introduce el codigo de la placa que deseas enviar a todos");
                return;
            }

            foreach (GameClient Client in QuasarEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().Username == Session.GetHabbo().Username)
                    continue;

                if (!Client.GetHabbo().GetBadgeComponent().HasBadge(Params[1]))
                {
                    Client.GetHabbo().GetBadgeComponent().GiveBadge(Params[1], true, Client);
                    Client.SendMessage(RoomNotificationComposer.SendBubble("cred", "" + Session.GetHabbo().Username + " te acaba de enviar la placa " + Params[1] + ".", ""));
                }
                else
                    Client.SendMessage(RoomNotificationComposer.SendBubble("cred", "" + Session.GetHabbo().Username + " ha intentado enviarte la placa " + Params[1] + " pero ya la tienes.", ""));
            }

            Session.SendWhisper("Usted le ha dado con exito a cada uno de los del hotel " + Params[1] + " placa!");
        }
    }
}
