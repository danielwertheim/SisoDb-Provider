using System;
using EnsureThat;

namespace SisoDb
{
    [Serializable]
    public abstract class SisoConnectionInfo : ISisoConnectionInfo
    {
        public abstract string DbName { get; }

        public StorageProviders ProviderType { get; private set; }

        public ParallelInsertMode ParallelInsertMode { get; private set; }

        public IConnectionString ClientConnectionString { get; private set; }

        public abstract IConnectionString ServerConnectionString { get; }

        protected SisoConnectionInfo(IConnectionString connectionString)
        {
            Ensure.That(connectionString, "connectionString").IsNotNull();

            ClientConnectionString = connectionString;

            ProviderType = (StorageProviders)Enum.Parse(typeof(StorageProviders), ClientConnectionString.Provider, true);

            if(!string.IsNullOrWhiteSpace(ClientConnectionString.ParallelInsertMode))
                ParallelInsertMode = (ParallelInsertMode)Enum.Parse(typeof(ParallelInsertMode), ClientConnectionString.ParallelInsertMode, true);
        }
    }
}