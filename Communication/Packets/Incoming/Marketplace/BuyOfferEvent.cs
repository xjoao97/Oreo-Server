using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;

using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Catalog.Marketplace;
using Quasar.Communication.Packets.Outgoing.Marketplace;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;

using Quasar.Database.Interfaces;


namespace Quasar.Communication.Packets.Incoming.Marketplace
{
    class BuyOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int OfferId = Packet.PopInt();

            DataRow Row = null;
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `state`,`timestamp`,`total_price`,`extra_data`,`item_id`,`furni_id`,`user_id`,`limited_number`,`limited_stack` FROM `catalog_marketplace_offers` WHERE `offer_id` = @OfferId LIMIT 1");
                dbClient.AddParameter("OfferId", OfferId);
                Row = dbClient.getRow();
            }

            if (Row == null)
            {
                this.ReloadOffers(Session);
                return;
            }

            if (Convert.ToString(Row["state"]) == "2")
            {
                Session.SendNotification("Oops, a oferta não está disponível!");
                this.ReloadOffers(Session);
                return;
            }

            if (QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().FormatTimestamp() > (Convert.ToDouble(Row["timestamp"])))
            {
                Session.SendNotification("Oops, esta oferta espirou..");
                this.ReloadOffers(Session);
                return;
            }

            ItemData Item = null;
            if (!QuasarEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["item_id"]), out Item))
            {
                Session.SendNotification("Este item não está disponível!");
                this.ReloadOffers(Session);
                return;
            }
            else
            {
                if (Convert.ToInt32(Row["user_id"]) == Session.GetHabbo().Id)
                {
                    Session.SendNotification("Você não pode comprar seu próprio item!");
                    return;
                }

                if (Convert.ToInt32(Row["total_price"]) > Session.GetHabbo().Duckets)
                {
                    Session.SendNotification("Oops, Você não tem duckets suficiente para essa oferta!");
                    return;
                }

                Session.GetHabbo().Duckets -= Convert.ToInt32(Row["total_price"]);
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Convert.ToInt32(Row["total_price"])));


                Item GiveItem = ItemFactory.CreateSingleItem(Item, Session.GetHabbo(), Convert.ToString(Row["extra_data"]), Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["furni_id"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]));
                if (GiveItem != null)
                {
                    Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                    Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));

                    Session.SendMessage(new Quasar.Communication.Packets.Outgoing.Catalog.PurchaseOKComposer());
                    Session.SendMessage(new FurniListAddComposer(GiveItem));
                    Session.SendMessage(new FurniListUpdateComposer());
                }

                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `catalog_marketplace_offers` SET `state` = '2' WHERE `offer_id` = '" + OfferId + "' LIMIT 1");

                    int Id = 0;
                    dbClient.SetQuery("SELECT `id` FROM catalog_marketplace_data WHERE sprite = " + Item.SpriteId + " LIMIT 1;");
                    Id = dbClient.getInteger();

                    if (Id > 0)
                        dbClient.RunQuery("UPDATE `catalog_marketplace_data` SET `sold` = `sold` + 1, `avgprice` = (avgprice + " + Convert.ToInt32(Row["total_price"]) + ") WHERE `id` = " + Id + " LIMIT 1;");
                    else
                        dbClient.RunQuery("INSERT INTO `catalog_marketplace_data` (sprite, sold, avgprice) VALUES ('" + Item.SpriteId + "', '1', '" + Convert.ToInt32(Row["total_price"]) + "')");


                    if (QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.ContainsKey(Item.SpriteId) && QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.ContainsKey(Item.SpriteId))
                    {
                        int num3 = QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts[Item.SpriteId];
                        int num4 = (QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages[Item.SpriteId] += Convert.ToInt32(Row["total_price"]));

                        QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Remove(Item.SpriteId);
                        QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Add(Item.SpriteId, num4);
                        QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Remove(Item.SpriteId);
                        QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Add(Item.SpriteId, num3 + 1);
                    }
                    else
                    {
                        if (!QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.ContainsKey(Item.SpriteId))
                            QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Add(Item.SpriteId, Convert.ToInt32(Row["total_price"]));

                        if (!QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.ContainsKey(Item.SpriteId))
                            QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Add(Item.SpriteId, 1);
                    }
                }
            }

            this.ReloadOffers(Session);
        }

        private void ReloadOffers(GameClient Session)
        {
            int MinCost = -1;
            int MaxCost = -1;
            string SearchQuery = "";
            int FilterMode = 1;

            DataTable table = null;
            StringBuilder builder = new StringBuilder();
            string str = "";
            builder.Append("WHERE state = '1' AND timestamp >= " + QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().FormatTimestampString());
            if (MinCost >= 0)
            {
                builder.Append(" AND total_price > " + MinCost);
            }
            if (MaxCost >= 0)
            {
                builder.Append(" AND total_price < " + MaxCost);
            }
            switch (FilterMode)
            {
                case 1:
                    str = "ORDER BY asking_price DESC";
                    break;

                default:
                    str = "ORDER BY asking_price ASC";
                    break;
            }

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {

                dbClient.SetQuery("SELECT `offer_id`,`item_type`,`sprite_id`,`total_price`,`limited_number`,`limited_stack` FROM `catalog_marketplace_offers` " + builder.ToString() + " " + str + " LIMIT 500");
                dbClient.AddParameter("search_query", "%" + SearchQuery + "%");
                if (SearchQuery.Length >= 1)
                {
                    builder.Append(" AND `public_name` LIKE @search_query");
                }
                table = dbClient.getTable();
            }

            QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems.Clear();
            QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Clear();
            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (!QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Contains(Convert.ToInt32(row["offer_id"])))
                    {
                        MarketOffer item = new MarketOffer(Convert.ToInt32(row["offer_id"]), Convert.ToInt32(row["sprite_id"]), Convert.ToInt32(row["total_price"]), int.Parse(row["item_type"].ToString()), Convert.ToInt32(row["limited_number"]), Convert.ToInt32(row["limited_stack"]));
                        QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Add(Convert.ToInt32(row["offer_id"]));
                        QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems.Add(item);
                    }
                }
            }

            Dictionary<int, MarketOffer> dictionary = new Dictionary<int, MarketOffer>();
            Dictionary<int, int> dictionary2 = new Dictionary<int, int>();

            foreach (MarketOffer item in QuasarEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems)
            {
                if (dictionary.ContainsKey(item.SpriteId))
                {
                    if (dictionary[item.SpriteId].TotalPrice > item.TotalPrice)
                    {
                        dictionary.Remove(item.SpriteId);
                        dictionary.Add(item.SpriteId, item);
                    }

                    int num = dictionary2[item.SpriteId];
                    dictionary2.Remove(item.SpriteId);
                    dictionary2.Add(item.SpriteId, num + 1);
                }
                else
                {
                    dictionary.Add(item.SpriteId, item);
                    dictionary2.Add(item.SpriteId, 1);
                }
            }

            Session.SendMessage(new MarketPlaceOffersComposer(MinCost, MaxCost, dictionary, dictionary2));
        }
    }
}
