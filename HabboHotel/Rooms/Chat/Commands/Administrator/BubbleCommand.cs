using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Rooms.Chat.Styles;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class BubbleCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_bubble"; }
        }

        public string Parameters
        {
            get { return "%id%"; }
        }

        public string Description
        {
            get { return "Use um balão de fala diferente!"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, digite o ID");
                return;
            }

            int Bubble = 0;
            if (!int.TryParse(Params[1].ToString(), out Bubble))
            {
                Session.SendWhisper("Por favor digite um número valido.");
                return;
            }

            ChatStyle Style = null;
            if (!QuasarEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Bubble, out Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(Style.RequiredRight)))
            {
                Session.SendWhisper("Oops, No puede utilizar esta burbuja por los permisos de rangos [ Raros: 32, 28]!");
                return;
            }

            User.LastBubble = Bubble;
            Session.GetHabbo().CustomBubbleId = Bubble;
            Session.SendWhisper("Balão alterado para: " + Bubble);
        }
    }
}
