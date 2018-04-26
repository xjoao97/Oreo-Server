using System.Data;
using MySql.Data.MySqlClient;

namespace Quasar.Database.Interfaces
{
    public interface IRegularQueryAdapter
    {
        void AddParameter(string name, object query);
        bool findsResult();
        int getInteger();
        DataRow getRow();
        string getString();
        DataTable getTable();
        void runFastQuery(string query);
		string GetCommand();
		MySqlDataReader ExecuteReader();
        void RunQuery(string query);
        void SetQuery(string query);
    }
}