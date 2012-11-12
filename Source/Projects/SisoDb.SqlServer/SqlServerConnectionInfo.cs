using System;
using System.Data.SqlClient;

namespace SisoDb.SqlServer
{
    [Serializable]
    public abstract class SqlServerConnectionInfo : SisoConnectionInfo
    {
        protected SqlServerConnectionInfo(StorageProviders providerType, string connectionStringOrName)
            : base(providerType, connectionStringOrName)
        {
        }

        protected override string OnFormatClientConnectionString(string connectionString)
        {
            connectionString = base.OnFormatClientConnectionString(connectionString);
            var cnStringBuilder = new SqlConnectionStringBuilder(connectionString) { MultipleActiveResultSets = true };
            return cnStringBuilder.ConnectionString;
        }

        protected override string OnFormatServerConnectionString(string connectionString)
        {
            connectionString = base.OnFormatServerConnectionString(connectionString);
            var cnStringBuilder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = string.Empty, MultipleActiveResultSets = true };
            return cnStringBuilder.ConnectionString;
        }

        protected override string OnExtractDbName(string connectionString)
        {
            var cnStringBuilder = new SqlConnectionStringBuilder(connectionString);
            return cnStringBuilder.InitialCatalog;
        }
    }
}