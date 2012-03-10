using System.Data;
using System.Data.SqlClient;

namespace SisoDb.Sql2008
{
	public class Sql2008ConnectionManager : ConnectionManagerBase, IConnectionManager
	{
        protected override IDbConnection GetConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}