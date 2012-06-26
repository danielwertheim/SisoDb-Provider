using System;
using System.Data;

namespace SisoDb
{
    public interface IConnectionManager
    {
        Func<IDbConnection, IDbConnection> OnConnectionCreated { get; set; }

        void ResetOnConnectionCreated();

        IDbConnection OpenServerConnection(ISisoConnectionInfo connectionInfo);
        IDbConnection OpenClientConnection(ISisoConnectionInfo connectionInfo);

        void ReleaseAllConnections();
        void ReleaseServerConnection(IDbConnection dbConnection);
        void ReleaseClientConnection(IDbConnection dbConnection);
    }
}