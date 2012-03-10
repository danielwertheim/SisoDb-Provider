using System.Data;
using System.Data.SqlClient;

namespace SisoDb.Sql2008
{
	public class Sql2008ConnectionManager : IConnectionManager
	{
        public virtual IDbConnection OpenServerConnection(ISisoConnectionInfo connectionInfo)
        {
            var cn = new SqlConnection(connectionInfo.ServerConnectionString.PlainString);
            cn.Open();

            return cn;
        }

        public virtual IDbConnection OpenClientDbConnection(ISisoConnectionInfo connectionInfo)
        {
            var cn = new SqlConnection(connectionInfo.ClientConnectionString.PlainString);
            cn.Open();

            return cn;
        }

        public virtual void ReleaseAllDbConnections() { }

        public virtual void ReleaseServerConnection(IDbConnection dbConnection)
        {
            if (dbConnection == null)
                return;

            dbConnection.Close();
            dbConnection.Dispose();
        }

        public virtual void ReleaseClientDbConnection(IDbConnection dbConnection)
        {
            if (dbConnection == null)
                return;

            dbConnection.Close();
            dbConnection.Dispose();
        }
	}
}