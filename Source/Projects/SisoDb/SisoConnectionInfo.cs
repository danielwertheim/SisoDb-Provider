using System;
using SisoDb.Configurations;
using SisoDb.EnsureThat;
using SisoDb.Resources;

namespace SisoDb
{
    [Serializable]
    public abstract class SisoConnectionInfo : ISisoConnectionInfo
    {
        public StorageProviders ProviderType { get; private set; }
        public string DbName { get; private set; }
        public string ClientConnectionString { get; private set; }
        public string ServerConnectionString { get; private set; }

        protected SisoConnectionInfo(StorageProviders providerType, string connectionStringOrName)
        {
            Ensure.That(connectionStringOrName, "connectionStringOrName").IsNotNull();

            ProviderType = providerType;
            var connectionString = GetConnectionString(connectionStringOrName);

            ClientConnectionString = FormatDbConnectionString(connectionString);
            ServerConnectionString = FormatServerConnectionString(connectionString);
            DbName = ExtractDbName(ClientConnectionString);
        }

        private string GetConnectionString(string connectionStringOrName)
        {
            var value = OnGetConnectionString(connectionStringOrName);
            if(string.IsNullOrWhiteSpace(value))
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_CouldNotLocateConnectionString);

            return value;
        }

        private string FormatDbConnectionString(string connectionString)
        {
            try
            {
                return OnFormatClientConnectionString(connectionString);
            }
            catch (Exception ex)
            {
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_CouldNotFormatDbConnectionString, new[] { ex });
            }
        }

        private string FormatServerConnectionString(string connectionString)
        {
            try
            {
                return OnFormatServerConnectionString(connectionString);
            }
            catch (Exception ex)
            {
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_CouldNotFormatServerConnectionString, new[] { ex });
            }
        }

        private string ExtractDbName(string connectionString)
        {
            var dbName = OnExtractDbName(connectionString);
            if (string.IsNullOrWhiteSpace(dbName))
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_MissingName);

            return dbName;
        }

        protected virtual string OnGetConnectionString(string connectionStringOrName)
        {
            return ConnectionString.Get(connectionStringOrName);
        }

        protected virtual string OnFormatClientConnectionString(string connectionString)
        {
            return connectionString;
        }

        protected virtual string OnFormatServerConnectionString(string connectionString)
        {
            return connectionString;
        }

        protected abstract string OnExtractDbName(string connectionString);
    }
}