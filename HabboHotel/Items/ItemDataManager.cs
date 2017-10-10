using System;
using System.Data;
using System.Collections.Generic;

using log4net;

using Quasar.Core;
using Quasar.Database.Interfaces;

namespace Quasar.HabboHotel.Items
{
    public class ItemDataManager
    {
        private static readonly ILog log = LogManager.GetLogger("Quasar.HabboHotel.Items.ItemDataManager");

        public Dictionary<int, ItemData> _items;
        public Dictionary<int, ItemData> _gifts;

        public ItemDataManager()
        {
            this._items = new Dictionary<int, ItemData>();
            this._gifts = new Dictionary<int, ItemData>();
        }

        public void Init()
        {
            if (this._items.Count > 0)
                this._items.Clear();

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `furniture`");
                DataTable ItemData = dbClient.getTable();

                if (ItemData != null)
                {
                    foreach (DataRow Row in ItemData.Rows)
                    {
                        try
                        {
                            int id = Convert.ToInt32(Row["id"]);
                            int spriteID = Convert.ToInt32(Row["sprite_id"]);
                            string itemName = Convert.ToString(Row["item_name"]);
                            string PublicName = Convert.ToString(Row["public_name"]);
                            string type = Row["type"].ToString();
                            int width = Convert.ToInt32(Row["width"]);
                            int length = Convert.ToInt32(Row["length"]);
                            double height = Convert.ToDouble(Row["stack_height"]);
                            bool allowStack = QuasarEnvironment.EnumToBool(Row["can_stack"].ToString());
                            bool allowWalk = QuasarEnvironment.EnumToBool(Row["is_walkable"].ToString());
                            bool allowSit = QuasarEnvironment.EnumToBool(Row["can_sit"].ToString());
                            bool allowRecycle = QuasarEnvironment.EnumToBool(Row["allow_recycle"].ToString());
                            bool allowTrade = QuasarEnvironment.EnumToBool(Row["allow_trade"].ToString());
                            bool allowMarketplace = Convert.ToInt32(Row["allow_marketplace_sell"]) == 1;
                            bool allowGift = Convert.ToInt32(Row["allow_gift"]) == 1;
                            bool allowInventoryStack = QuasarEnvironment.EnumToBool(Row["allow_inventory_stack"].ToString());
                            InteractionType interactionType = InteractionTypes.GetTypeFromString(Convert.ToString(Row["interaction_type"]));
                            int cycleCount = Convert.ToInt32(Row["interaction_modes_count"]);
                            string vendingIDS = Convert.ToString(Row["vending_ids"]);
                            string heightAdjustable = Convert.ToString(Row["height_adjustable"]);
                            int EffectId = Convert.ToInt32(Row["effect_id"]);
                            int WiredId = Convert.ToInt32(Row["wired_id"]);
                            bool IsRare = QuasarEnvironment.EnumToBool(Row["is_rare"].ToString());
                            int ClothingId = Convert.ToInt32(Row["clothing_id"]);
                            bool ExtraRot = QuasarEnvironment.EnumToBool(Row["extra_rot"].ToString());
                            int SongID = Convert.ToInt32(Row["song_Id"]);

                            if (!this._gifts.ContainsKey(spriteID))
                                this._gifts.Add(spriteID, new ItemData(id, spriteID, itemName, PublicName, type, width, length, height, allowStack, allowWalk, allowSit, allowRecycle, allowTrade, allowMarketplace, allowGift, allowInventoryStack, interactionType, cycleCount, vendingIDS, heightAdjustable, EffectId, WiredId, IsRare, ClothingId, ExtraRot, SongID));

                            if (!this._items.ContainsKey(id))
                                this._items.Add(id, new ItemData(id, spriteID, itemName, PublicName, type, width, length, height, allowStack, allowWalk, allowSit, allowRecycle, allowTrade, allowMarketplace, allowGift, allowInventoryStack, interactionType, cycleCount, vendingIDS, heightAdjustable, EffectId, WiredId, IsRare, ClothingId, ExtraRot, SongID));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadKey();
                            Logging.WriteLine("Could not load item #" + Convert.ToInt32(Row[0]) + ", please verify the data is okay.");
                        }
                    }
                }
            }
          //log.Info(">> Items Manager -> Ligado");
        }

        public bool GetItem(int Id, out ItemData Item)
        {
            if (this._items.TryGetValue(Id, out Item))
                return true;
            return false;
        }

        internal ItemData GetItemByName(string name)
        {
            foreach (KeyValuePair<int, ItemData> entry in _items)
            {
                ItemData item = entry.Value;
                if (item.ItemName == name)
                    return item;
            }
            return null;
        }

        public bool GetGift(int SpriteId, out ItemData Item)
        {
            if (this._gifts.TryGetValue(SpriteId, out Item))
                return true;
            return false;
        }
    }
}