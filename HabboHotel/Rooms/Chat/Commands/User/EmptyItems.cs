using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class EmptyItems : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_empty_items"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Apague permanentemente todo o seu inventário"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.GetHabbo().GetInventoryComponent().ClearItems();
                Session.SendMessage(new RoomNotificationComposer("Aviso",
                 "Você apagou todos os mobis do inventário!", "nothing", ""));
                return;
            }
            else
            {
            }
        }
    }
}