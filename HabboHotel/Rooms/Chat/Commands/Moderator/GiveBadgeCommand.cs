using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveBadgeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_give_badge"; }
        }

        public string Parameters
        {
            get { return "Usuário Código"; }
        }

        public string Description
        {
            get { return "Dê um Emblema para algum usuário"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length != 3)
            {
                Session.SendWhisper("Algo deu errado! Tente novamente", 1);
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient != null)
            {
                if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge(Params[2]))
                {
                    TargetClient.GetHabbo().GetBadgeComponent().GiveBadge(Params[2], true, TargetClient);
                    TargetClient.SendMessage(new RoomCustomizedAlertComposer("${wiredfurni.badgereceived.body}"));
                    if (TargetClient.GetHabbo().Id != Session.GetHabbo().Id) ;
                }
                else
                    Session.SendMessage(new RoomCustomizedAlertComposer("Parece que esse usuário já tem este Emblema. Veja seu Inventário!"));
                return;
            }
            else
            {
                Session.SendWhisper("Algo deu errado! Tente novamente", 1);
                return;
            }
        }
    }
}