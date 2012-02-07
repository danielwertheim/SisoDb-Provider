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

        public ParallelInserts ParallelInserts { get; private set; }

        public IConnectionString ConnectionString { get; private set; }

        public abstract IConnectionString ServerConnectionString { get; }

        protected SisoConnectionInfo(string connectionStringOrName) 
            : this(GetConnectionString(connectionStringOrName))
        { }

        protected SisoConnectionInfo(IConnectionString connectionString)
        {
            Ensure.That(connectionString, "connectionString").IsNotNull();

            ConnectionString = connectionString;

            ProviderType = (StorageProviders)Enum.Parse(typeof(StorageProviders), ConnectionString.Provider, true);

            if(!string.IsNullOrWhiteSpace(ConnectionString.ParallelInserts))
                ParallelInserts = (ParallelInserts)Enum.Parse(typeof(ParallelInserts), ConnectionString.ParallelInserts, true);
        }

        protected static IConnectionString GetConnectionString(string connectionStringOrName)
        {
            Ensure.That(connectionStringOrName, "connectionStringOrName").IsNotNullOrWhiteSpace();

			var config = ConfigurationManager.ConnectionStrings[string.Concat(Environment.MachineName, "_", connectionStringOrName)];
        	config = config ?? ConfigurationManager.ConnectionStrings[connectionStringOrName];

            return config == null ? new ConnectionString(connectionStringOrName) : new ConnectionString(config.ConnectionString);
        }
    }
}