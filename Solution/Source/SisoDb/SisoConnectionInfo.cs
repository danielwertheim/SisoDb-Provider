using System;
using System.Configuration;
using SisoDb.Resources;

namespace SisoDb
{
    [Serializable]
    public class SisoConnectionInfo : ISisoConnectionInfo
    {
        public StorageProviders ProviderType { get; private set; }

        public IConnectionString ConnectionString { get; private set; }

        public SisoConnectionInfo(string connectionStringOrName)
        {
            if (string.IsNullOrWhiteSpace(connectionStringOrName))
                throw new ArgumentNullException("connectionStringOrName", ExceptionMessages.SisoConnectionInfo_MissingConnectionStringOrNameArg);

            var config = ConfigurationManager.ConnectionStrings[connectionStringOrName];
            var cnString = config == null ? new ConnectionString(connectionStringOrName) : new ConnectionString(config.ConnectionString);

            Initialize(cnString);
        }

        public SisoConnectionInfo(IConnectionString connectionString)
        {
            if(connectionString == null)
                throw new ArgumentNullException("connectionString");

            Initialize(connectionString);
        }

        private void Initialize(IConnectionString connectionString)
        {
            ProviderType = (StorageProviders)Enum.Parse(typeof(StorageProviders), connectionString.Provider, true);
            ConnectionString = connectionString;
        }
    }
}