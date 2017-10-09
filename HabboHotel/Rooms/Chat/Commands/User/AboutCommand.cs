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
            Session.SendMessage(new RoomNotificationComposer("Plus r4.5 - BUILD 050917:",
            "<font color='#31B404'><b>About Quasar:</b>\n" +
            "<font size=\"11\" color=\"#1C1C1C\">Private project powered by Custom and enhanced by Salinas & Root, </font>" +
            "<font size=\"11\" color=\"#1C1C1C\">our main goal is sharing Habbo basics with our customers, adding some untold content.\n\nQuasar Project started on July 2016 and keeps up for being one of the most relevant projects over the community.\n\n" +
            "<font size =\"12\" color=\"#0B4C5F\"><b>Stats:</b></font>\n" +
            "<font size =\"11\" color=\"#1C1C1C\">  <b> · Users: </b> " + OnlineUsers + "\r  <b> · Rooms: </b> " + RoomCount + "\r  <b> · Uptime: </b> " + Uptime.Days + " days, " + Uptime.Hours + " hours and " + Uptime.Minutes + " minutes.\r <b>  · Date: </b> " + dtDateTime + ".</font>\n\n" +
            "Last update on  <b>5-09-2017</b>.\n\n<font size =\"12\" color=\"#0B4C5F\">Check out <b> :changelog</b> for last updates.</font>", "quas"));
        }
    }
}