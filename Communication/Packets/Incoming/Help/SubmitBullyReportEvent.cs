using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Help;

namespace Quasar.Communication.Packets.Incoming.Help
{
    class SubmitBullyReportEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            //0 = sent, 1 = blocked, 2 = no chat, 3 = already reported.
            if (Session == null)
                return;

            int UserId = Packet.PopInt();
            if (UserId == Session.GetHabbo().Id)//Hax
                return;

            if (Session.GetHabbo().AdvertisingReportedBlocked)
            {
                Session.SendMessage(new SubmitBullyReportComposer(1));
                return;
            }

            GameClient Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(Convert.ToInt32(UserId));
            if (Client == null)
            {
                Session.SendMessage(new SubmitBullyReportComposer(0));
                return;
            }

            if (Session.GetHabbo().LastAdvertiseReport > QuasarEnvironment.GetUnixTimestamp())
            {
                Session.SendNotification("Você pode enviar um relato a cada 5 minutos!");
                return;
            }

            if (Client.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendNotification("Neste momento, os membros da equipe não podem ser informados através desta ferramenta.");
                return;
            }

            if (!Client.GetHabbo().HasSpoken)
            {
                Session.SendMessage(new SubmitBullyReportComposer(2));
                return;
            }

            if (Client.GetHabbo().AdvertisingReported && Session.GetHabbo().Rank < 2)
            {
                Session.SendMessage(new SubmitBullyReportComposer(3));
                return;
            }

            if (Session.GetHabbo().Rank <= 1)
                Session.GetHabbo().LastAdvertiseReport = QuasarEnvironment.GetUnixTimestamp() + 300;
            else
                Session.GetHabbo().LastAdvertiseReport = QuasarEnvironment.GetUnixTimestamp();

            Client.GetHabbo().AdvertisingReported = true;
            Session.SendMessage(new SubmitBullyReportComposer(0));
            //QuasarEnvironment.GetGame().GetClientManager().ModAlert("Novo relatório! " + Client.GetHabbo().Username + " reportou a publicidade de " + Session.GetHabbo().Username +".");
            QuasarEnvironment.GetGame().GetClientManager().DoAdvertisingReport(Session, Client);
            return;
        }
    }
}
