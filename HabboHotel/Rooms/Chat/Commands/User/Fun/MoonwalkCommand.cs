namespace Quasar.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class MoonwalkCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_moonwalk"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ande de costas igual Michael Jackson"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            var user = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (user != null)
                if (user.CurrentEffect != 136)
                    user.ApplyEffect(136);
                else
                    user.ApplyEffect(0);
        }
    }
}