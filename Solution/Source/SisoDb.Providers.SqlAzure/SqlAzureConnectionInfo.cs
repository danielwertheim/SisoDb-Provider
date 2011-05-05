using System;
using SisoDb.Providers.Sql2008;
using SisoDb.Core;
using SisoDb.Providers.SqlAzure.Resources;

namespace SisoDb.Providers.SqlAzure
{
    [Serializable]
    public class SqlAzureConnectionInfo : Sql2008ConnectionInfo
    {
        public SqlAzureConnectionInfo(ISisoConnectionInfo connectionInfo) : base(connectionInfo)
        {
        }

        protected override void OnEnsureValid()
        {
            if (ProviderType != StorageProviders.SqlAzure)
                throw new SisoDbException(SqlAzureExceptions.AzureDatabase_UnsupportedProviderSpecified
                    .Inject(ProviderType, StorageProviders.SqlAzure));
        }
    }
}