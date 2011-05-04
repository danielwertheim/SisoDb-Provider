using System;
using SisoDb.Core;
using SisoDb.Providers.Sql2008Provider;
using SisoDb.Resources;

namespace SisoDb.Providers.AzureProvider
{
    public class AzureDatabase : Sql2008Database
    {
        public AzureDatabase(ISisoConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo.AssertNotNull("connectionInfo");

            if(ConnectionInfo.ProviderType != StorageProviders.SqlAzure)
                throw new SisoDbException(ExceptionMessages.AzureDatabase_UnsupportedProviderSpecified
                    .Inject(ConnectionInfo.ProviderType, StorageProviders.SqlAzure));

            OnInitialize();
        }

        public override void EnsureNewDatabase()
        {
            throw new NotSupportedException();
        }

        public override void CreateIfNotExists()
        {
            throw new NotSupportedException();
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