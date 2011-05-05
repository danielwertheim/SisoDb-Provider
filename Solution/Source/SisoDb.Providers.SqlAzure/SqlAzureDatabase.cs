using System;
using SisoDb.Core;
using SisoDb.Providers.Sql2008;
using SisoDb.Providers.SqlAzure.Resources;

namespace SisoDb.Providers.SqlAzure
{
    public class SqlAzureDatabase : Sql2008Database
    {
        internal SqlAzureDatabase(ISisoConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo.AssertNotNull("connectionInfo");

            if(ConnectionInfo.ProviderType != StorageProviders.SqlAzure)
                throw new SisoDbException(SqlAzureExceptions.AzureDatabase_UnsupportedProviderSpecified
                    .Inject(ConnectionInfo.ProviderType, StorageProviders.SqlAzure));

            Initialize();
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