using Quasar.Communication.Packets.Outgoing;
using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Communication.Packets.Outgoing.Rooms.Polls;
using Quasar.HabboHotel.Rooms.Polls;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class MassPollCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_masspoll"; }
        }

        public string Parameters
        {
            get { return "%id%"; }
        }

        public string Description
        {
            get { return "Envia uma votação para o hotel."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor digite o ID da Poll.");
                return;
            }

            RoomPoll poll = null;
            if (QuasarEnvironment.GetGame().GetPollManager().TryGetPollForHotel(int.Parse(Params[1]), out poll))
            {
                if (poll.Type == RoomPollType.Poll)
                {
                    QuasarEnvironment.GetGame().GetClientManager().SendMessage(new PollOfferComposer(poll));
                }
            }
            return;
        }
    }
}
