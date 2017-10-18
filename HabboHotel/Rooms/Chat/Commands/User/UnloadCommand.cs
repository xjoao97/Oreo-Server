using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Outgoing.Rooms.Session;

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
            get { return "id"; }
        }

        public string Description
        {
            get { return "Recarrega o quarto"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetHabbo().GetPermissions().HasRight("room_unload_any"))
            {
                Room r = null;
                if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Room.Id, out r))
                {
                    return;
                }

                List<RoomUser> UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();
                QuasarEnvironment.GetGame().GetRoomManager().UnloadRoom(r, true);

                foreach (RoomUser User in UsersToReturn)
                {
                    if (User != null)
                    {
                        User.GetClient().SendMessage(new RoomForwardComposer(Room.Id));
                    }
                }
            }
            else
            {
                if (Room.CheckRights(Session, true))
                {
                    List<RoomUser> UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();
                    QuasarEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);

                    foreach (RoomUser User in UsersToReturn)
                    {
                        if (User != null)
                        {
                            User.GetClient().SendMessage(new RoomForwardComposer(Room.Id));
                        }
                    }
                }
            }
        }
    }
}
