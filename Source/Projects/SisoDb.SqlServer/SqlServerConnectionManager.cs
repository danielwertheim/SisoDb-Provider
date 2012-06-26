using System;
using System.Data;
using EnsureThat;
using SisoDb.Dac;

namespace SisoDb.SqlServer
{
    public class SqlServerConnectionManager : IConnectionManager
    {
        protected readonly IAdoDriver Driver;

        public SqlServerConnectionManager(IAdoDriver driver)
        {
            Ensure.That(driver, "driver").IsNotNull();

            Driver = driver;
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
            var cn = OnConnectionCreated(Driver.CreateConnection(connectionInfo.ServerConnectionString.PlainString));
            cn.Open();
            return cn;
        }

        public virtual IDbConnection OpenClientConnection(ISisoConnectionInfo connectionInfo)
        {
            var cn = OnConnectionCreated(Driver.CreateConnection(connectionInfo.ClientConnectionString.PlainString));
            cn.Open();
            return cn;
        }

        public virtual void ReleaseAllConnections() { }

        public virtual void ReleaseServerConnection(IDbConnection dbConnection)
        {
            if (dbConnection == null)
                return;

            dbConnection.Close();
            dbConnection.Dispose();
        }

        public virtual void ReleaseClientConnection(IDbConnection dbConnection)
        {
            if (dbConnection == null)
                return;

            dbConnection.Close();
            dbConnection.Dispose();
        }
    }
}
