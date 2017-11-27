using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Utilities;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Outgoing.Handshake;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class FlagMeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_flagme"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Mude seu nome de usuário."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!this.CanChangeName(Session.GetHabbo()))
            {
                Session.SendWhisper("Desculpe, parece que atualmente você não tem a opção de alterar seu nome de usuário, espere mais um pouco.");
                return;
            }

            Session.GetHabbo().ChangingName = true;
            Session.SendNotification("Por favor, lembre-se que se seu nome for impróprio você será banido.\r\rObserve que você não mudará seu nome novamente se tiver problemas com o que você escolheu\r\rFeche esta janela e clique em si mesmo para começar a renomear!");
            Session.SendMessage(new UserObjectComposer(Session.GetHabbo()));
        }

        private bool CanChangeName(Habbo Habbo)
        {
            if (Habbo.Rank == 1 && Habbo.VIPRank == 0 && Habbo.LastNameChange == 0)
                return true;
            else if (Habbo.Rank == 2 && Habbo.VIPRank == 1 && (Habbo.LastNameChange == 0 || (QuasarEnvironment.GetUnixTimestamp() + 604800) > Habbo.LastNameChange))
                return true;
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 2 && (Habbo.LastNameChange == 0 || (QuasarEnvironment.GetUnixTimestamp() + 86400) > Habbo.LastNameChange))
                return true;
            else if (Habbo.Rank == 1 && Habbo.VIPRank == 3)
                return true;
            else if (Habbo.GetPermissions().HasRight("mod_tool"))
                return true;

            return false;
        }
    }
}
