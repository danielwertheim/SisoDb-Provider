using System;
using SisoDb.Core;
using SisoDb.Sql2008;
using SisoDb.SqlAzure.Resources;

namespace SisoDb.SqlAzure
{
    [Serializable]
    public class SqlAzureConnectionInfo : SqlConnectionInfo
    {
        public SqlAzureConnectionInfo(string connectionStringOrName) 
            : this(new SisoConnectionInfo(connectionStringOrName))
        {}

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