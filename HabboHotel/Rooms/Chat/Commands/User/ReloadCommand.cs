using System.Linq;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Rooms.Session;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class Reloadcommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_reload"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Recarregar o Quarto"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetHabbo().Id != Room.OwnerId && !Session.GetHabbo().GetPermissions().HasRight("room_any_owner"))
            {
                Session.SendWhisper("Apenas o proprietário deste quarto pode usar esse comando", 1);
                return;
            }

            List<RoomUser> UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();
            QuasarEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);

            foreach (RoomUser User in UsersToReturn)
            {
                if (User == null || User.GetClient() == null)
                    continue;

                User.GetClient().SendMessage(new RoomForwardComposer(Room.Id));
            }

        }
    }
}