using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Communication.Packets.Outgoing.Messenger;

namespace Quasar.Communication.Packets.Incoming.Messenger
{
    class SendMsgEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            int userId = Packet.PopInt();
            if (userId == 0 || userId == Session.GetHabbo().Id)
                return;

            string message = Packet.PopString();
            if (string.IsNullOrWhiteSpace(message)) return;
            if (Session.GetHabbo().TimeMuted > 15)
            {
                Session.SendWhisper("Oops, você foi silenciado por 15 segundos.");
                return;
            }

            if (message.Contains("&#1Âº;") || message.Contains("&#1Âº") || message.Contains("&#"))
            { Session.SendMessage(new MassEventComposer("habbopages/spammer.txt")); return; }

            string word;
            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                QuasarEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(message, out word))
            {
                Session.GetHabbo().BannedPhraseCount++;
                if (Session.GetHabbo().BannedPhraseCount >= 1)
                {
                    Session.GetHabbo().TimeMuted = 15;
                    Session.SendNotification("Você mencionou uma palavra do filtro " + QuasarEnvironment.GetDBConfig().DBData["hotel.name"] + ", talvez seja um erro. Aguarte um momento, esse sistema serve para evitar divulgação. Lembre-se de que você apenas avisou os membros da equipe, se não é um caso de divulgação, não entre em pânico. Aviso" + Session.GetHabbo().BannedPhraseCount + " / 5");
                    QuasarEnvironment.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, "Divulgação: " + Session.GetHabbo().Username + " / Frase: " + message + " / Palavra: " + word.ToUpper() + " / Frase: " + Session.GetHabbo().BannedPhraseCount + " / 5."));
                    QuasarEnvironment.GetGame().GetClientManager().StaffAlert2(new RoomNotificationComposer("Alerta de publicista:",
                    "<b><font color=\"#B40404\">Por favor, investigue antes de tomar uma atitude.</font></b><br><br>Palavra: <b>" + word.ToUpper() + "</b>.<br><br><b>Frase:</b><br><i>" + message +
                    "</i>.<br><br><b>Tipo:</b><br>Chat de sala.\r\n" + "<b>Usuário: " + Session.GetHabbo().Username + "</b><br><b>Sequência:</b> " + Session.GetHabbo().BannedPhraseCount + "/ 5.", "foto", "Investigar", "event:navigator/goto/" +
                    Session.GetHabbo().CurrentRoomId));
                    return;
                }
                if (Session.GetHabbo().BannedPhraseCount >= 5)
                {
                    QuasarEnvironment.GetGame().GetModerationManager().BanUser("Quasar", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "Você foi banido por spam (" + message + ")", (QuasarEnvironment.GetUnixTimestamp() + 78892200));
                    Session.Disconnect();
                    return;
                }
                return;
            }

            Session.GetHabbo().GetMessenger().SendInstantMessage(userId, message);

        }
    }
}
