
using Oreo.Communication.Packets.Outgoing.Rooms.Notifications;
using Oreo.HabboHotel.Rooms;
using Oreo.HabboHotel.GameClients;
using Oreo.Communication.Packets.Outgoing.Rooms.Engine;
using System.Linq;
using Oreo.Communication.Packets.Outgoing.Inventory.Purse;
 
namespace Oreo.HabboHotel.Rooms.Chat.Commands.Events
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
                return "%message%";
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
                    foreach (GameClient client in OreoServer.GetGame().GetClientManager().GetClients.ToList())
                        if (client.GetHabbo().AllowEvents == true)
                        {
                            string Message = CommandManager.MergeParams(Params, 1);
 
                            client.SendMessage(new RoomNotificationComposer("Está acontecendo um evento!",
                                 "Que tal você participar e receber uma premiação e um incrível emblema gamer? <br><br>O evento tem a ideia de proporcionar entretenimento para todos os usuários do hotel!<br><br>O evento se chama:<b>  " + Message +
                                 "</b><br>Promovido por:<b>  " + Session.GetHabbo().Username +
                                 "</b> <br><br>Caso tenha vontade de participar, clique no botão abaixo. <br><br>Se não gosta de receber as notificações é só digitar<b>  :alertas</b> !",
                                 "events", "Participar", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                        }
                        else
                        {
                            client.SendWhisper("Parece que está havendo um novo evento em nosso hotel. Para reativar as mensagens de eventos digite ;alertas", 1);
                        }
                }
            }
        }
    }
}