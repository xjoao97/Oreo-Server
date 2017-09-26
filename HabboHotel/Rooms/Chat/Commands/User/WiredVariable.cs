using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class WiredVariable : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_stats"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Lista de variables en tu WIRED: Mensaje."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
           
            Session.SendMessage(new MassEventComposer("habbopages/chat/wiredvars.txt"));
            return;

        }
    }
}
