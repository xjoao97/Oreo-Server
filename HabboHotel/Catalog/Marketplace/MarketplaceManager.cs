using System;
using System.Collections.Generic;

using Quasar.Database.Interfaces;

namespace Quasar.HabboHotel.Catalog.Marketplace
{
    public class MarketplaceManager
    {
        public List<int> MarketItemKeys = new List<int>();
        public List<MarketOffer> MarketItems = new List<MarketOffer>();
        public Dictionary<int, int> MarketCounts = new Dictionary<int, int>();
        public Dictionary<int, int> MarketAverages = new Dictionary<int, int>();

        public MarketplaceManager()
        {

        }

        public int AvgPriceForSprite(int SpriteID)
        {
            int num = 0;
            int num2 = 0;
            if (this.MarketAverages.ContainsKey(SpriteID) && this.MarketCounts.ContainsKey(SpriteID))
            {
                if (this.MarketCounts[SpriteID] > 0)
                {
                    return (this.MarketAverages[SpriteID] / this.MarketCounts[SpriteID]);
                }
                return 0;
            }

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `avgprice` FROM `catalog_marketplace_data` WHERE `sprite` = '" + SpriteID + "' LIMIT 1");
                num = dbClient.getInteger();

                dbClient.SetQuery("SELECT `sold` FROM `catalog_marketplace_data` WHERE `sprite` = '" + SpriteID + "' LIMIT 1");
                num2 = dbClient.getInteger();
            }

            this.MarketAverages.Add(SpriteID, num);
            this.MarketCounts.Add(SpriteID, num2);

            if (num2 > 0)
                return Convert.ToInt32(Math.Ceiling((double)(num / num2)));
            
            return 0;
        }

        public string FormatTimestampString()
        {
            return this.FormatTimestamp().ToString().Split(new char[] { ',' })[0];
        }

        public double FormatTimestamp()
        {
            return (QuasarEnvironment.GetUnixTimestamp() - 172800.0);
        }

        public int OfferCountForSprite(int SpriteID)
        {
            Dictionary<int, MarketOffer> dictionary = new Dictionary<int, MarketOffer>();
            Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
            foreach (MarketOffer item in this.MarketItems)
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
            if (dictionary2.ContainsKey(SpriteID))
            {
                return dictionary2[SpriteID];
            }
            return 0;
        }

        public int CalculateComissionPrice(float SellingPrice)
        {
            return Convert.ToInt32(Math.Ceiling(SellingPrice / 100 * 1));
        }
    }
}