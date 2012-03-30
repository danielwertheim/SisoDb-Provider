using System;
using System.Data;

namespace SisoDb
{
    public abstract class ConnectionManagerBase : IConnectionManager
    {

        public ConnectionManagerBase()
        {
            OnConnectionCreated = con => con;
        }

        public virtual Func<IDbConnection, IDbConnection> OnConnectionCreated { get; set; }

        public virtual IDbConnection OpenServerConnection(ISisoConnectionInfo connectionInfo)
        {
            var cn = OnConnectionCreated(CreateConnection(connectionInfo.ServerConnectionString.PlainString));
            cn.Open();

            return cn;
        }

        public virtual IDbConnection OpenClientDbConnection(ISisoConnectionInfo connectionInfo)
        {
            var cn = OnConnectionCreated(CreateConnection(connectionInfo.ClientConnectionString.PlainString));
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

        protected abstract IDbConnection CreateConnection(string connectionString);

    }
}
