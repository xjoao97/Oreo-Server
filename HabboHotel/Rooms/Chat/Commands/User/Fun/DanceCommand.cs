using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Outgoing.Rooms.Avatar;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class DanceCommand :IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_dance"; }
        }

        public string Parameters
        {
            get { return "%DanceId%"; }
        }

        public string Description
        {
            get { return "Dance de 0 a 4."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o id da dança de 0 a 4.");
                return;
            }

            int DanceId;
            if (int.TryParse(Params[1], out DanceId))
            {
                if (DanceId > 4 || DanceId < 0)
                {
                    Session.SendWhisper("Digite o id da dança de 0 a 4");
                    return;
                }

                Session.GetHabbo().CurrentRoom.SendMessage(new DanceComposer(ThisUser, DanceId));
            }
            else
                Session.SendWhisper("Digite um id válido de 0 a 4");
        }
    }
}
