using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableDiagonalCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_disable_diagonal"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Desative a opção de caminhar diagonalmente em seu quarto."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, Somente o proprietário da sala pode executar o comando!");
                return;
            }

            Room.GetGameMap().DiagonalEnabled = !Room.GetGameMap().DiagonalEnabled;
            Session.SendWhisper("Ninguém pode caminhar na diagonal da sala");
        }
    }
}
