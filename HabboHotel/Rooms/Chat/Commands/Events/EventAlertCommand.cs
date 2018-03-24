using Galaxy.Communication.Packets.Outgoing.Rooms.Notifications;
using Galaxy.HabboHotel.GameClients;
using System.Linq;
using System;
namespace Galaxy.HabboHotel.Rooms.Chat.Commands.Events
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
            get
            {
                return "[MENSAGEM]";
            }
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
            if (Session != null)
            {
                if (Room != null)
                {
                    if (Params.Length == 1)
                    {
                        Session.SendWhisper("Por favor, digite uma mensagem para enviar.");
                        return;
                    }
                    foreach (GameClient client in GalaxyServer.GetGame().GetClientManager().GetClients.ToList())
                    {
                        string Message = CommandManager.MergeParams(Params, 1);
                        client.SendMessage(new RoomNotificationComposer("Está acontecendo um evento!",
                                 "Está acontecendo um novo evento realizado pela equipe Space! <br><br>Este, tem o intuito de proporcionar um entretenimento a mais para os usuários!<br><br>Evento:<b>  " + Message +
                                 "</b><br>Por:<b>  " + Session.GetHabbo().Username +
                                 "</b> <br><br>Caso deseje participar, clique no botão abaixo! <br>",
                                 "events", "Participar do Evento", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));

                    }                  
                }
            }
        }
    }
}