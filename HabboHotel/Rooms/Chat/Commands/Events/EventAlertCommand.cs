using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class EventAlertCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_event_alert";
            }
        }
        public string Parameters
        {
            get { return "Mensagem"; }
        }
        public string Description
        {
            get
            {
                return "Enviar um alerta de evento";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            string Message = CommandManager.MergeParams(Params, 1);

            Room Quarto = Session.GetHabbo().CurrentRoom;
            string nomequarto = Quarto.Name;

            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Temos um Evento",
                             "<b>" + Session.GetHabbo().Username + "</b> está realizando um evento agora!<br><br><i>O evento vai ser: " + Message + "</i></b><br><br><font color=\"#7E7E7E\">Participe do evento e concorra a prêmios.</font>",
                             "eventswulles", "" + nomequarto + "", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
        }
    }
}