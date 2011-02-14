using System;
using SisoDb.Providers.SqlProvider;
using SisoDb.Resources;

namespace SisoDb.Providers.AzureProvider
{
    public class AzureDatabase : SqlDatabase
    {
        public AzureDatabase(ISisoConnectionInfo connectionInfo) : base(connectionInfo)
        {
            if(connectionInfo.ProviderType != StorageProviders.SqlAzure)
                throw new SisoDbException(ExceptionMessages.AzureDatabase_UnsupportedProviderSpecified
                    .Inject(connectionInfo.ProviderType, StorageProviders.SqlAzure));
        }

        public override void EnsureNewDatabase()
        {
            throw new NotSupportedException();
        }

        public override void CreateIfNotExists()
        {
            throw new NotSupportedException();
        }

        public override void InitializeExisting()
        {
            using (var client = new SqlDbClient(ConnectionInfo, false))
            {
                if (!client.DatabaseExists(Name))
                    throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(Name));

                client.CreateSysTables(Name);
            }
        }

        public override void DeleteIfExists()
        {
            throw new NotSupportedException();
        }

        public override bool Exists()
        {
            throw new NotSupportedException();
        }
    }
}