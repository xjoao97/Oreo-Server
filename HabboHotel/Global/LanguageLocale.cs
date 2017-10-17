﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;


using log4net;
using Quasar.Database.Interfaces;

namespace Quasar.HabboHotel.Global
{
    public class LanguageLocale
    {
        private Dictionary<string, string> _values = new Dictionary<string, string>();

        private static readonly ILog log = LogManager.GetLogger("Quasar.HabboHotel.Global.LanguageLocale");

        public LanguageLocale()
        {
            this._values = new Dictionary<string, string>();

            this.Init();
        }

        internal static string Value(string v, object p)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            if (this._values.Count > 0)
                this._values.Clear();

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `server_locale`");
                DataTable Table = dbClient.getTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        this._values.Add(Row["key"].ToString(), Row["value"].ToString());
                    }
                }
            }

            //log.Info(">> Language Manager -> Ligado");
        }

        public string TryGetValue(string value)
        {
            return this._values.ContainsKey(value) ? this._values[value] : "Missing language locale for [" + value + "]";
        }
    }
}
