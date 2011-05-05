using System;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlStrings;

namespace SisoDb.Providers.Dac
{
    public interface ISqlServerClient : IDisposable
    {
        StorageProviders ProviderType { get; }

        IConnectionString ConnectionString { get; }

        IDbDataTypeTranslator DbDataTypeTranslator { get; }

        ISqlStringsRepository SqlStringsRepository { get; }
        
        bool DatabaseExists(string name);

        void CreateDatabase(string name);

        void InitializeExistingDb(string name);

        void DropDatabase(string name);
    }
}