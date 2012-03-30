using System;
using System.Data;
using EnsureThat;

namespace SisoDb
{
    public abstract class ConnectionManagerBase : IConnectionManager
    {
        protected ConnectionManagerBase()
        {
            OnConnectionCreated = cn => cn;
        }

        private Func<IDbConnection, IDbConnection> _onConnectionCreated;

        public Func<IDbConnection, IDbConnection> OnConnectionCreated
        {
            get { return _onConnectionCreated; }
            set
            {
                Ensure.That(value, "OnConnectionCreated").IsNotNull();
                _onConnectionCreated = value;
            }
        }

        public virtual void ResetOnConnectionCreated()
        {
            OnConnectionCreated = cn => cn;
        }

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
