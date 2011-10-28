using System;
using SisoDb.Providers;

namespace SisoDb.Dac
{
    public interface IServerClient : IDisposable
    {
        StorageProviders ProviderType { get; }
        IConnectionString ConnectionString { get; }
        ISqlStatements SqlStatements { get; }
        bool DatabaseExists(string name);
        void CreateDatabase(string name);
        void InitializeExistingDb(string name);
        void DropDatabaseIfExists(string name);
    }
}