using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class UserCommandsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_commands"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Wulles ama rola"; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {

            Session.SendMessage(new MassEventComposer("habbopages/chat/commands/index.html"));
            return;

        }
    }
}