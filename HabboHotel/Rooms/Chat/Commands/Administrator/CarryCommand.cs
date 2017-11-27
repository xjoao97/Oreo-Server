using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class CarryCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_carry"; }
        }

        public string Parameters
        {
            get { return "%ItemId%"; }
        }

        public string Description
        {
            get { return "Pegue uma bebida"; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            int ItemId = 0;
            if (!int.TryParse(Convert.ToString(Params[1]), out ItemId))
            {
                Session.SendWhisper("Digite um id válido");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.CarryItem(ItemId);
        }
    }
}
