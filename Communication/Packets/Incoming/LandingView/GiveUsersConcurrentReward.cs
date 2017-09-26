using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.LandingView
{
    class GiveConcurrentUsersReward : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetStats().PurchaseUsersConcurrent)
            {
                Session.SendMessage(new RoomCustomizedAlertComposer("Você recebeu um prêmio!"));
            }

            string badge = QuasarEnvironment.GetDBConfig().DBData["usersconcurrent_badge"];
            int pixeles = int.Parse(QuasarEnvironment.GetDBConfig().DBData["usersconcurrent_pixeles"]);

            Session.GetHabbo().GOTWPoints = Session.GetHabbo().GOTWPoints + pixeles;
            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, pixeles, 103));
            Session.GetHabbo().GetBadgeComponent().GiveBadge(badge, true, Session);
            Session.SendMessage(new RoomCustomizedAlertComposer("Você recebeu um prêmio!"));
            Session.GetHabbo().GetStats().PurchaseUsersConcurrent = true;
        }
    }
}
