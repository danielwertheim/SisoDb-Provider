using System.Data;
using System.Data.SqlClient;

namespace SisoDb.Sql2012
{
    public class Sql2012ConnectionManager : ConnectionManagerBase
    {
        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}