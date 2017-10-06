using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class SendNotificationAlert : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_hotel_alert"; }
        }

        public string Parameters
        {
            get { return "%message%"; }
        }

        public string Description
        {
            get { return "Alerta para o Hotel inteiro"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Algo deu errado! Tente novamente", 1);
                return;
            }
            string Message = CommandManager.MergeParams(Params, 1);
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new RoomCustomizedAlertComposer(Message));

            return;
        }
    }
}