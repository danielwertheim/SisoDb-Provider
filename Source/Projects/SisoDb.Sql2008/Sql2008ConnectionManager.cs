using System.Data;
using System.Data.SqlClient;

namespace SisoDb.Sql2008
{
	public class Sql2008ConnectionManager : IConnectionManager
	{
		public IDbConnection OpenServerConnection(IConnectionString connectionString)
		{
			var cn = new SqlConnection(connectionString.PlainString);
			cn.Open();

			return cn;
		}

		public void ReleaseServerConnection(IDbConnection dbConnection)
		{
			if (dbConnection == null)
				return;

            dbConnection.Close();
			dbConnection.Dispose();
		}

		public IDbConnection OpenDbConnection(IConnectionString connectionString)
		{
			var cn = new SqlConnection(connectionString.PlainString);
			cn.Open();

			return cn;
		}

		public void ReleaseAllDbConnections() { }

		public void ReleaseDbConnection(IDbConnection dbConnection)
		{
			if (dbConnection == null)
				return;

            dbConnection.Close();
			dbConnection.Dispose();
		}
	}
}