using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SisoDb
{
    public abstract class ConnectionManagerBase : IConnectionManager
    {
    
        public virtual System.Data.IDbConnection OpenServerConnection(ISisoConnectionInfo connectionInfo)
        {
            var cn = GetConnection(connectionInfo.ServerConnectionString.PlainString);
            OpenConnection(cn);

            return cn;
        }


        public virtual System.Data.IDbConnection OpenClientDbConnection(ISisoConnectionInfo connectionInfo)
        {
            var cn = GetConnection(connectionInfo.ClientConnectionString.PlainString);
            OpenConnection(cn);

            return cn;
        }

        public virtual void ReleaseAllDbConnections() { }

        public virtual void ReleaseServerConnection(System.Data.IDbConnection dbConnection)
        {
            if (dbConnection == null)
                return;

            dbConnection.Close();
            dbConnection.Dispose();
        }

        public virtual void ReleaseClientDbConnection(System.Data.IDbConnection dbConnection)
        {
            if (dbConnection == null)
                return;

            dbConnection.Close();
            dbConnection.Dispose();
        }

        protected abstract IDbConnection GetConnection(string connectionString);
        protected virtual void OpenConnection(IDbConnection connection)
        {
            connection.Open();
        }

    }
}
