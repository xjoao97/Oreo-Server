using Quasar.Communication.Packets.Outgoing.Moderation;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class HALCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_hal"; }
        }

        public string Parameters
        {
            get { return "%message%"; }
        }

        public string Description
        {
            get { return "Envia um link para o hotel"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 2)
            {
                Session.SendWhisper("Por favor escreva a mensagem.");
                return;
            }

            string URL = Params[1];
            string Message = CommandManager.MergeParams(Params, 2);

            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new SendHotelAlertLinkEventComposer("Alerta da equipe:\r\n" + Message + "\r\n-" + Session.GetHabbo().Username, URL));
            return;
        }
    }
}
