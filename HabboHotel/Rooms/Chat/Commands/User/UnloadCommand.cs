using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class UnloadCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_unload"; }
        }

        public string Parameters
        {
            get { return "%id%"; }
        }

        public string Description
        {
            get { return "Recargar la sala en la que te encuentras."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetHabbo().GetPermissions().HasRight("room_unload_any"))
            {
                Room R = null;
                if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Room.Id, out R))
                    return;

                QuasarEnvironment.GetGame().GetRoomManager().UnloadRoom(R, true);
            }
            else
            {
                if (Room.CheckRights(Session, true))
                {
                    QuasarEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);
                }
            }
        }
    }
}
