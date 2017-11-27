using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableSpamCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_disable_spam"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Habilite ou desabilite a capacidade de receber spam."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Session.GetHabbo().AllowMessengerInvites = false;
            Session.SendWhisper("Você " + (Session.GetHabbo().AllowMessengerInvites ? "recebe" : "não recebe mais") + " Spam no console!");
        }
    }
}
