using System.Data;
using System.Data.SqlClient;

namespace SisoDb.Sql2008
{
	public class Sql2008ConnectionManager : ConnectionManagerBase
	{
        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}