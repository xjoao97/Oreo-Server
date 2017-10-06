using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;

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
            get { return "%message%"; }
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

            Session.GetHabbo()._eventsopened++;

            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Temos um Evento",
                             "<b>" + Session.GetHabbo().Username + "</b> está realizando um evento agora!<br><br><i>O evento vai ser: " + Message + "</i></b><br><br><font color=\"#7E7E7E\">Participe do evento e concorra a prêmios.</font>",
                             "eventswulles", "" + nomequarto + "", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));

            QuasarEnvironment.GetGame().GetClientManager().SendEventType2(new WhisperComposer(-1, "Novo evento: Para participar, <font color=\"#2E9AFE\"><a href='event:navigator/goto/" + Session.GetHabbo().CurrentRoomId + "'><b>clique aqui</b></a></font>.", 0, 33));
            QuasarEnvironment.GetGame().GetClientManager().SendEventType2(new WhisperComposer(-1, Message, 0, 33));
            QuasarEnvironment.GetGame().GetClientManager().SendEventType2(new WhisperComposer(-1, "Evento organizado por: " + Session.GetHabbo().Username + ".", 0, 33));
        }
    }
}