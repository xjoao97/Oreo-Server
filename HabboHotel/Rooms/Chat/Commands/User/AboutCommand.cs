using System;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class AboutCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_about"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Sobre o Emulador deste Hotel"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(QuasarEnvironment.GetUnixTimestamp()).ToLocalTime();

            TimeSpan Uptime = DateTime.Now - QuasarEnvironment.ServerStarted;
            int OnlineUsers = QuasarEnvironment.GetGame().GetClientManager().Count;
            int RoomCount = QuasarEnvironment.GetGame().GetRoomManager().Count;
            Session.SendMessage(new RoomNotificationComposer("Emulador R-5:",
            "<font size=\"12\" color=\"#1C1C1C\"><b>Informações do Servidor:</b>\n" +
            "<font size=\"12\" color=\"#1C1C1C\">Projeto privado capaz de realizar as funções básicas para um Hotel baseado em <b> Butterfly</b> e <b>PlusEmu</b></font>\n\n" +
            "<font size=\"12\" color=\"#1C1C1C\"><b>Desenvolvedor:</b> Arfeu\n\n" +
            "<font size=\"12\" color=\"#1C1C1C\"><b>Liberação de uso:</b> FPanel\n" +
            "<font size=\"12\" color=\"#1C1C1C\"><b>Build:</b> Tifany\n\n" +
            "<font size=\"12\" color=\"#1C1C1C\"><b>Agradecimentos:</b> Sledmore, 123, Joopie e Tweenty, Ghostman, CoolMemes e Igor G\n\n", "emula"));
        }
    }
}