using System;
using System.Data.SqlClient;
using SisoDb.Resources;

namespace SisoDb.SqlServer
{
    [Serializable]
    public abstract class SqlServerConnectionInfo : SisoConnectionInfo
    {
        protected SqlServerConnectionInfo(StorageProviders providerType, IConnectionString connectionString)
            : base(providerType, connectionString)
        {
            if (string.IsNullOrWhiteSpace(DbName))
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_MissingName);
        }

        protected override IConnectionString OnFormatServerConnectionString(IConnectionString connectionString)
        {
            var cnString = base.OnFormatServerConnectionString(connectionString);
            var cnStringBuilder = new SqlConnectionStringBuilder(cnString.PlainString) { InitialCatalog = string.Empty };

            return cnString.ReplacePlain(cnStringBuilder.ConnectionString);
        }

        protected override string OnExtractDbName(IConnectionString connectionString)
        {
            var cnStringBuilder = new SqlConnectionStringBuilder(connectionString.PlainString);
            return cnStringBuilder.InitialCatalog;
        }
    }
}