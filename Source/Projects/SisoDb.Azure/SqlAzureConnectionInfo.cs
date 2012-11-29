using System;
using SisoDb.SqlServer;

namespace SisoDb.Azure
{
    [Serializable]
    public class SqlAzureConnectionInfo : SqlServerConnectionInfo
    {
        public SqlAzureConnectionInfo(string connectionStringOrName)
            : base(StorageProviders.SqlAzure, connectionStringOrName)
        { }
    }
}