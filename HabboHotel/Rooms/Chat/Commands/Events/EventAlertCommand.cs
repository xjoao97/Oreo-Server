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
                return "Abre un evento en todo el hotel.";
            }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            string Message = CommandManager.MergeParams(Params, 1);

            Session.GetHabbo()._eventsopened++;

            QuasarEnvironment.GetGame().GetClientManager().SendEventType1(new RoomNotificationComposer("¡Nuevo evento!", "¡<b><font color=\"#2E9AFE\">" + Session.GetHabbo().Username + "</font></b> está organizando un nuevo evento en este momento! Si quieres ganar <font color=\"#f18914\"><b>  " + QuasarEnvironment.GetDBConfig().DBData["seasonal.currency.name"] + "</b></font> participa ahora mismo.<br><br>¿Quieres participar en este juego? ¡Haz click en el botón inferior de <b> Ir a la sala del evento</b>, y dentro podrás participar, sigue las instrucciones!<br><br>¿De qué trata este evento?<br><br><font color='#FF0040'><b>"
              + Message + "</b></font><br><br>¡Te esperamos con los brazos abiertos!<br><br>Recuerda que puedes cambiar esta alerta con el comando <b>:eventtype</b> .", "eventoshabbi", "¡Ir a la sala del evento!", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));

            QuasarEnvironment.GetGame().GetClientManager().SendEventType2(new WhisperComposer(-1, "Hay un nuevo evento, haz <font color=\"#2E9AFE\"><a href='event:navigator/goto/" + Session.GetHabbo().CurrentRoomId + "'><b>click aquí</b></a></font> para ir al evento.", 0, 33));
            QuasarEnvironment.GetGame().GetClientManager().SendEventType2(new WhisperComposer(-1, Message, 0, 33));
            QuasarEnvironment.GetGame().GetClientManager().SendEventType2(new WhisperComposer(-1, "Evento organizado por: " + Session.GetHabbo().Username + ".", 0, 33));
        }


    }
}

