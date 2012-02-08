using System.Data;
using System.Data.SqlClient;

namespace SisoDb.Sql2012
{
	public class Sql2012ConnectionManager : IConnectionManager
	{
		public IDbConnection OpenServerConnection(ISisoConnectionInfo connectionInfo)
		{
            var cn = new SqlConnection(connectionInfo.ServerConnectionString.PlainString);
			cn.Open();

			return cn;
		}

	    public IDbConnection OpenClientDbConnection(ISisoConnectionInfo connectionInfo)
		{
			var cn = new SqlConnection(connectionInfo.ClientConnectionString.PlainString);
			cn.Open();

			return cn;
		}

	    public void ReleaseAllDbConnections() { }

	    public void ReleaseServerConnection(IDbConnection dbConnection)
	    {
	        if (dbConnection == null)
	            return;

	        dbConnection.Close();
	        dbConnection.Dispose();
	    }

	    public void ReleaseClientDbConnection(IDbConnection dbConnection)
		{
			if (dbConnection == null)
				return;

            dbConnection.Close();
			dbConnection.Dispose();
		}
	}
}