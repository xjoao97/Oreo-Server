using System.Linq;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RoomBadgeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_room_badge"; }
        }

        public string Parameters
        {
            get { return "Código"; }
        }

        public string Description
        {
            get { return "Dar um emblema para todos do Quarto"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Você precisa por o código do emblema que deseja dar", 1);
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                    continue;

                if (!User.GetClient().GetHabbo().GetBadgeComponent().HasBadge(Params[1]))
                {
                    User.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(Params[1], true, User.GetClient());
                    User.GetClient().SendMessage(new RoomCustomizedAlertComposer("${wiredfurni.badgereceived.body}"));
                }
                else
                    User.GetClient().SendMessage(new RoomCustomizedAlertComposer("Parece que você já tem esse Emblema. Veja seu Inventário!"));
            }

            Session.SendWhisper("Você enviou o emblema: " + Params[2] + ", para todos deste Quarto", 1);
        }
    }
}
