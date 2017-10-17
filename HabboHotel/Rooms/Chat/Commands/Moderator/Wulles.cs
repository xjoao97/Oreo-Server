using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class Wulles : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_hotel_alert"; }
        }

        public string Parameters
        {
            get { return "Wulles"; }
        }

        public string Description
        {
            get { return "Wulles"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Algo deu errado! Tente novamente", 1);
                return;
            }

            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new AlertNotificationHCMessageComposer(1));
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new AlertNotificationHCMessageComposer(2));
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new AlertNotificationHCMessageComposer(3));
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new AlertNotificationHCMessageComposer(4));
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new AlertNotificationHCMessageComposer(5));
            return;

        }
    }
}