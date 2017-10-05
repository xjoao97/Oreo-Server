using Quasar.HabboHotel.Rooms.Chat.Styles;
using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class SetBetCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_bubble"; }
        }

        public string Parameters
        {
            get { return "%diamantes%"; }
        }

        public string Description
        {
            get { return "Use para apostar nas máquinas de cassino."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve inserir um valor até 50!", 34);
                return;
            }

            int Bet = 0;
            if (!int.TryParse(Params[1].ToString(), out Bet))
            {
                Session.SendWhisper("Por favor, digite um valor válido!", 34);
                return;
            }

            Session.GetHabbo()._bet = Bet;
            Session.SendWhisper("Agora você está apostando " + Bet + " diamantes!", 34);
        }
    }
}
