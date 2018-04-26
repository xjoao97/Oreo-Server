﻿using System;
using MySql.Data.MySqlClient;
using Quasar.Core;
using Quasar.Database.Interfaces;

namespace Quasar.Database
{
    public sealed class DatabaseManager
    {
        private readonly string _connectionStr;

        public DatabaseManager(string ConnectionStr)
        {
            this._connectionStr = ConnectionStr;
        }

        public bool IsConnected()
        {
            try
            {
                MySqlConnection Con = new MySqlConnection(this._connectionStr);
                Con.Open();
                MySqlCommand CMD = Con.CreateCommand();
                CMD.CommandText = "SELECT 1+1";
                CMD.ExecuteNonQuery();

                CMD.Dispose();
                Con.Close();
            }
            catch (MySqlException ex)
            {
				Console.WriteLine(ex);
                return false;
            }

            return true;
        }

        public IQueryAdapter GetQueryReactor()
        {
            try
            {
                IDatabaseClient DbConnection = new DatabaseConnection(this._connectionStr);

                DbConnection.Connect();

                return DbConnection.GetQueryReactor();
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
                return null;
            }
        }
    }
}