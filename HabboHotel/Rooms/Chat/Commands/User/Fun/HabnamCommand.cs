namespace Quasar.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class HabnamCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_habnam"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Oppa Gangnam Style"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            var user = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (user != null)
                if (user.CurrentEffect != 140)
                    user.ApplyEffect(140);
                else
                    user.ApplyEffect(0);
        }
    }
}