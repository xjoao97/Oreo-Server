﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Support;
using Quasar.HabboHotel.Rooms.Chat.Moderation;
using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.HabboHotel.Moderation;

namespace Quasar.Communication.Packets.Incoming.Moderation
{
    class ReportCameraPhotoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            //if (QuasarEnvironment.GetGame().GetModerationManager().(Session.GetHabbo().Id))
            //{
            //    Session.SendMessage(new BroadcastMessageAlertComposer("Você atualmente possui um ticket pendente, aguarde uma resposta de um moderador."));
            //    return;
            //}

            int photoId;

            if (!int.TryParse(Packet.PopString(), out photoId))
            {
                return;
            }

            int roomId = Packet.PopInt();
            int creatorId = Packet.PopInt();
            int categoryId = Packet.PopInt();

           // QuasarEnvironment.GetGame().GetModerationTool().SendNewTicket(Session, categoryId, creatorId, "", new List<string>(), (int) ModerationSupportTicketType.PHOTO, photoId);
            QuasarEnvironment.GetGame().GetClientManager().ModAlert("Foi enviado um novo ticket de suporte!");
        }
    }
}
