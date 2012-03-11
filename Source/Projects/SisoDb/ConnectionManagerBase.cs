using System.Data;

namespace SisoDb
{
    public abstract class ConnectionManagerBase : IConnectionManager
    {
        public virtual IDbConnection OpenServerConnection(ISisoConnectionInfo connectionInfo)
        {
            var cn = CreateConnection(connectionInfo.ServerConnectionString.PlainString);
            OnOpenConnection(cn);

            return cn;
        }

        public virtual IDbConnection OpenClientDbConnection(ISisoConnectionInfo connectionInfo)
        {
            var cn = CreateConnection(connectionInfo.ClientConnectionString.PlainString);
            OnOpenConnection(cn);

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

        protected abstract IDbConnection CreateConnection(string connectionString);

        protected virtual void OnOpenConnection(IDbConnection connection)
        {
            connection.Open();
        }
    }
}
