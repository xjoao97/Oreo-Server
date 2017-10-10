using Quasar.Communication.Packets.Outgoing.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class SendGraphicAlertCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_hotel_alert"; }
        }

        public string Parameters
        {
            get { return "Nome da Imagem"; }
        }

        public string Description
        {
            get { return "Enviar uma Imagem para todo o Hotel"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Algo deu errado! Tente novamente", 1);
                return;
            }

            string image = Params[1];
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new GraphicAlertComposer(image));

            return;
        }
    }
}