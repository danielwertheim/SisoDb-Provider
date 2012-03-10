using System;
using System.Data;
namespace SisoDb
{
    public interface ISisoConnectionInfo
    {
        string DbName { get; }

        StorageProviders ProviderType { get; }

        BackgroundIndexing BackgroundIndexing { get; }

        IConnectionString ClientConnectionString { get; }

        IConnectionString ServerConnectionString { get; }

        Func<IDbConnection, IDbConnection> OnConnectionCreated { get; set; }
    }
}