﻿using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;


namespace Quasar.HabboHotel.Catalog.Clothing
{
    public class ClothingManager
    {
        private Dictionary<int, ClothingItem> _clothing;

        public ClothingManager()
        {
            this._clothing = new Dictionary<int, ClothingItem>();
           
            this.Init();
        }

        public void Init()
        {
            if (this._clothing.Count > 0)
                this._clothing.Clear();

            
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`clothing_name`,`clothing_parts` FROM `catalog_clothing`");
                    using (var reader = dbClient.ExecuteReader())
                    while (reader.Read())
                        _clothing.Add(reader.GetInt32("id"), new ClothingItem(reader.GetInt32("id"), reader.GetString("clothing_name"), reader.GetString("clothing_parts")));
            }
        }

        public bool TryGetClothing(int ItemId, out ClothingItem Clothing)
        {
            if (this._clothing.TryGetValue(ItemId, out Clothing))
                return true;
            return false;
        }
    }
}
