using System;
using System.Configuration;
using EnsureThat;

namespace SisoDb
{
    [Serializable]
    public abstract class SisoConnectionInfo : ISisoConnectionInfo
    {
        public abstract string DbName { get; }

        public StorageProviders ProviderType { get; private set; }

        public IConnectionString ConnectionString { get; private set; }

        public abstract IConnectionString ServerConnectionString { get; }

        protected SisoConnectionInfo(string connectionStringOrName)
        {
            Ensure.That(() => connectionStringOrName).IsNotNullOrWhiteSpace();

            ConnectionString = GetConnectionString(connectionStringOrName);
            ProviderType = (StorageProviders)Enum.Parse(typeof(StorageProviders), ConnectionString.Provider, true);
        }

        private static IConnectionString GetConnectionString(string connectionStringOrName)
        {
            var config = ConfigurationManager.ConnectionStrings[connectionStringOrName];

            return config == null ? new ConnectionString(connectionStringOrName) : new ConnectionString(config.ConnectionString);
        }
    }
}