using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.Global;
using Quasar.HabboHotel.Catalog;
using Quasar.Communication.Packets.Outgoing;

namespace Quasar.Communication.Packets.Incoming.Inventory.Purse
{
   class GetHabboClubWindowEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int _page = 5;

            if (Session.GetHabbo().lastLayout.Equals("loyalty_vip_buy"))
                _page = int.Parse(QuasarEnvironment.GetDBConfig().DBData["catalog.hcbuy.id"]);

            CatalogPage page = null;
            if (!QuasarEnvironment.GetGame().GetCatalog().TryGetPage(_page, out page))
            
                return;

            ServerPacket Message = new ServerPacket(ServerPacketHeader.GetClubComposer);
            Message.WriteInteger(page.Items.Values.Count);

            foreach (CatalogItem catalogItem in page.Items.Values)
            {
                catalogItem.SerializeClub(Message, Session);
            }

            Message.WriteInteger(Packet.PopInt());

            Session.SendMessage(Message);
        }
    }
}
