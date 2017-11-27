using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Notifications;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Communication.Packets.Outgoing.Moderation;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    class CheckGnomeNameEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int Id = Packet.PopInt();
            string Pin = Packet.PopString();

            if (Pin == Session.GetHabbo().PinClient)
            {
                QuasarEnvironment.GetGame().GetClientManager().ManagerAlert(RoomNotificationComposer.SendBubble("frank_tiny", "Um membro da equipe acabou de entrar no hotel: " + Session.GetHabbo().Username + ".", ""));
                Session.SendMessage(new CheckGnomeNameComposer(Pin, 0));
                Session.SendMessage(new GraphicAlertComposer("stafflogin"));
                Session.GetHabbo().StaffOk = true;
                if (Session.GetHabbo().GetPermissions().HasRight("mod_tickets"))
                {
                    Session.SendMessage(new ModeratorInitComposer(
                      QuasarEnvironment.GetGame().GetModerationManager().UserMessagePresets,
                      QuasarEnvironment.GetGame().GetModerationManager().RoomMessagePresets,
                      QuasarEnvironment.GetGame().GetModerationManager().GetTickets));
                }
            }
            else
            {
                Session.SendMessage(new CheckGnomeNameComposer(Pin, 0));
                Session.SendMessage(new RoomCustomizedAlertComposer("AVISO: O pin está incorreto!"));
                QuasarEnvironment.GetGame().GetClientManager().ManagerAlert(new MOTDNotificationComposer("Atenção: O Staff " + Session.GetHabbo().Username + " inseriu o seu pin incorreto, vá até ele para verificar sua identidade!"));
                Session.GetConnection().Dispose();
                this.LogCommand(Session.GetHabbo().Id, "Sessão inválida", Session.GetHabbo().MachineId, Session.GetHabbo().Username);
            }
        }

        public void LogCommand(int UserId, string Data, string MachineId, string Username)
        {
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `logs_client_staff` (`user_id`,`data_string`,`machine_id`, `timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                dbClient.AddParameter("UserId", UserId);
                dbClient.AddParameter("Data", Data);
                dbClient.AddParameter("MachineId", MachineId);
                dbClient.AddParameter("Timestamp", QuasarEnvironment.GetUnixTimestamp());
                dbClient.RunQuery();
            }
        }
    }
}
