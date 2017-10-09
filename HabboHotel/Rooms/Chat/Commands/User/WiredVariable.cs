using Quasar.Communication.Packets.Outgoing.Notifications;
using Quasar.HabboHotel.GameClients;
using System.Text;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class WiredVariable : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_stats"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Veja comandos especiais para usar no jogo"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            StringBuilder List = new StringBuilder("");
            List.AppendLine(" Comandos para EFEITO WIRED: Mostrar mensagem:");
            List.AppendLine(" %sit% - Faz o usuário sentar.");
            List.AppendLine(" %lay% - Faz o usuário deitar.");
            List.AppendLine(" %stand% - Faz o usuário levantar.");
            List.AppendLine(" %user% - Mostra o nome do usuário.");
            List.AppendLine(" %userid% - Mostra o ID de registro do usuário.");
            List.AppendLine(" %honor% - Mostra Pontos de Honra do usuário.");
            List.AppendLine(" %duckets% - Mostra Duckets do usuário.");
            List.AppendLine(" %diamonds% - Mostra Diamantes do usuário.");
            List.AppendLine(" %credits% - Mostra Moedas do usuário.");
            List.AppendLine(" %rank% - Mostra o Cargo do usuário.");
            List.AppendLine(" %roomname% - Mostra o Nome do Quarto.");
            List.AppendLine(" %roomusers% - Mostra quantos usuários tem no Quarto.");
            List.AppendLine(" %roomlikes% - Mostra quantar Curtidas o Quarto tem.");
            List.AppendLine(" %roomowner% - Mostra o Nome do Dono do Quarto.");
            List.AppendLine(" %userson% - Mostra a quantidade de usuários no hotel em tempo real.");
            Session.SendMessage(new MOTDNotificationComposer(List.ToString()));

        }
    }
}