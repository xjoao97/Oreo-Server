using Quasar.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class ViewInventaryCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_viewinventary"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Permite ver o inventário de um usuário"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Room == null)
                return;

            if (Params.Length == 2)
            {
                string Username = Params[1];

                GameClient Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
                if (Client != null)
                {
                    Session.SendWhisper("O usuário está online, espere ele sair do hotel!");
                    return;
                }

                int UserId = QuasarEnvironment.GetGame().GetClientManager().GetUserIdByUsername(Username);
                if (UserId == 0)
                {
                    Session.SendWhisper("O nome de usuário não existe.");
                    return;
                }

                Session.GetHabbo().GetInventoryComponent().LoadUserInventory(UserId);

                Session.SendWhisper("O inventário foi alterado para o" + Username);
            }
            else
            {
                Session.GetHabbo().GetInventoryComponent().LoadUserInventory(0);

                Session.SendWhisper("Seu inventário voltou ao normal.");
            }

            //Session.SendWhisper("...");
        }
    }
}
