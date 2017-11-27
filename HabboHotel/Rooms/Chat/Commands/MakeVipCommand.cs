using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands
{
    internal class MakeVipCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_makevip"; }
        }

        public string Parameters
        {
            get { return "%username% %days%"; }
        }

        public string Description
        {
            get { return "Dê VIP um a usuário"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário para quem você enviará");
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro, aparentemente o usuário não está sendo alcançado ou não está conectado");
                return;
            }

            int Days = int.Parse(CommandManager.MergeParams(Params, 2));

            if (Days > 31)
            {
                Session.SendWhisper("Ocorreu um erro, você não pode enviar tantos dias para o mesmo usuário.");
                return;
            }

            TargetClient.GetHabbo().GetClubManager().AddOrExtendSubscription("club_vip", Days * 24 * 3600, Session);
            TargetClient.SendMessage(new AlertNotificationHCMessageComposer(4));
        }
    }
}
