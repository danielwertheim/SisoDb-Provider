using System.Data;
using System.Data.SqlClient;

namespace SisoDb.Sql2012
{
    public class Sql2012ConnectionManager : ConnectionManagerBase, IConnectionManager
    {
        protected override IDbConnection GetConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }

}