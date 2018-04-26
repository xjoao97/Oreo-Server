using System;
using System.Data;

using Quasar.Communication.Packets.Outgoing.Marketplace;
using Quasar.Database.Interfaces;

namespace Quasar.Communication.Packets.Incoming.Marketplace
{
    class GetMarketplaceItemStatsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            int SpriteId = Packet.PopInt();


            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `avgprice` FROM `catalog_marketplace_data` WHERE `sprite` = @SpriteId LIMIT 1");
                dbClient.AddParameter("SpriteId", SpriteId);
				using (var reader = dbClient.ExecuteReader())
                if (reader.Read())
                Session.SendMessage(new MarketplaceItemStatsComposer(ItemId, SpriteId, reader.GetInt32("avgprice")));
                else
                Session.SendMessage(new MarketplaceItemStatsComposer(ItemId, SpriteId, 0));
            }

        }
    }
}