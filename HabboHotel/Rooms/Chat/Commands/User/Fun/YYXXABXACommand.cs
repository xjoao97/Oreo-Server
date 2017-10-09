namespace Quasar.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class YYXXABXACommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_yyxxabxa"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ganhe um Sabre de Luz"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            var user = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (user != null)
                if (user.CurrentEffect != 196)
                    user.ApplyEffect(196);
                else
                    user.ApplyEffect(0);
        }
    }
}