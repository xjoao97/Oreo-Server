using System;
using MySql.Data.MySqlClient;

namespace Quasar.Database.Interfaces
{
    public interface IDatabaseClient : IDisposable
    {
        void Connect();
        void Disconnect();
        IQueryAdapter GetQueryReactor();
        MySqlCommand CreateNewCommand();
        void ReportDone();
    }
}