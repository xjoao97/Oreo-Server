using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;
using Quasar.Utilities;
using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RemoveBadgeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_remove_badge"; }
        }

        public string Parameters
        {
            get { return "%username% %badge%"; }
        }

        public string Description
        {
            get { return "Remove o emblema de alguém"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 3)
            {
                GameClient TargetClient = null;
                TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
                if (TargetClient != null)
                    if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge(Params[2]))
                    {
                        {
                            Session.SendNotification("Este usuário não possui o emblema " + Params[2] + "");
                        }
                    }
                    else
                    {
                        RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                        TargetClient.GetHabbo().GetBadgeComponent().RemoveBadge(Params[2], TargetClient);
                        TargetClient.SendNotification("Seu emblema " + Params[2] + " foi removido pelo Staff " + ThisUser.GetUsername() + "!");
                        Session.SendNotification("O emblema foi removido!");

                    }
            }
            else
            {
                Session.SendNotification("Usuario não encontrado.");
                return;
            }
        }
    }
}
