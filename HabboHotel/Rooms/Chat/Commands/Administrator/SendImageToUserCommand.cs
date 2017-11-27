using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class SendImageToUserCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_alert_user"; }
        }

        public string Parameters
        {
            get { return "%usuario% %imagem%"; }
        }

        public string Description
        {
            get { return "Envia uma mensagem para alguém"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor digite o nome do usuário.");
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Oops, o usuário não foi encontrado ou não está online.");
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Oops, o usuário não foi encontrado ou não está online.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 2);

            TargetClient.SendMessage(new GraphicAlertComposer(Message));
            Session.SendWhisper("Alerta enviado para " + TargetClient.GetHabbo().Username +".");

        }
    }
}
