using System;
using EnsureThat;
using NCore;
using SisoDb.Resources;

namespace SisoDb
{
    [Serializable]
    public abstract class SisoConnectionInfo : ISisoConnectionInfo
    {
        public abstract string DbName { get; }

        public StorageProviders ProviderType { get; private set; }

        public BackgroundIndexing BackgroundIndexing { get; private set; }

        public IConnectionString ClientConnectionString { get; private set; }

        public abstract IConnectionString ServerConnectionString { get; }

        protected SisoConnectionInfo(IConnectionString connectionString)
        {
            Ensure.That(connectionString, "connectionString").IsNotNull();

            ClientConnectionString = connectionString;

            ProviderType = (StorageProviders)Enum.Parse(typeof(StorageProviders), ClientConnectionString.Provider, true);

            if (!string.IsNullOrWhiteSpace(ClientConnectionString.BackgroundIndexing))
                BackgroundIndexing = (BackgroundIndexing)Enum.Parse(typeof(BackgroundIndexing), ClientConnectionString.BackgroundIndexing, true);

            if (BackgroundIndexing != BackgroundIndexing.Off)
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_BackgroundIndexingNotSupported.Inject(ProviderType));
        }
    }
}