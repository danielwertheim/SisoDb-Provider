using System;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Resources;

namespace SisoDb
{
    [Serializable]
    public abstract class SisoConnectionInfo : ISisoConnectionInfo
    {
        public StorageProviders ProviderType { get; private set; }

        public string DbName { get; private set; }

        public BackgroundIndexing BackgroundIndexing { get; private set; }

        public IConnectionString ClientConnectionString { get; private set; }

        public IConnectionString ServerConnectionString { get; private set; }

        protected SisoConnectionInfo(StorageProviders providerType, IConnectionString connectionString)
        {
            Ensure.That(connectionString, "connectionString").IsNotNull();

            ProviderType = providerType;
            EnsureCorrectProviderIfItExists(connectionString);

            ClientConnectionString = FormatConnectionString(connectionString);
            ServerConnectionString = FormatServerConnectionString(connectionString);
            BackgroundIndexing = ExtractBackgroundIndexing(ClientConnectionString);
            DbName = ExtractDbName(ClientConnectionString);

            if (BackgroundIndexing != BackgroundIndexing.Off)
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_BackgroundIndexingNotSupported.Inject(ProviderType));
        }

        private void EnsureCorrectProviderIfItExists(IConnectionString connectionString)
        {
            var providerType = ExtractProviderType(connectionString);
            if (providerType == null)
                return;

            if (providerType != ProviderType)
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_UnsupportedProviderSpecified.Inject(providerType, ProviderType));
        }

        private IConnectionString FormatConnectionString(IConnectionString connectionString)
        {
            try
            {
                return OnFormatConnectionString(connectionString);
            }
            catch (Exception ex)
            {
                throw new SisoDbException("Could not parse sent connection string. If connection string name is passed. Ensure it has match in config-file. Inspect inner exception for more details.", new[] { ex });
            }
        }

        private IConnectionString FormatServerConnectionString(IConnectionString connectionString)
        {
            try
            {
                return OnFormatServerConnectionString(connectionString);
            }
            catch (Exception ex)
            {
                throw new SisoDbException("Could not parse sent server connection string. If connection string name is passed. Ensure it has match in config-file. Inspect inner exception for more details.", new[] { ex });
            }
        }

        private StorageProviders? ExtractProviderType(IConnectionString connectionString)
        {
            return OnExtractProviderType(connectionString);
        }

        private BackgroundIndexing ExtractBackgroundIndexing(IConnectionString connectionString)
        {
            return OnExtractBackgroundIndexing(connectionString);
        }

        protected string ExtractDbName(IConnectionString connectionString)
        {
            return OnExtractDbName(connectionString);
        }

        protected virtual IConnectionString OnFormatConnectionString(IConnectionString connectionString)
        {
            return connectionString;
        }

        protected virtual IConnectionString OnFormatServerConnectionString(IConnectionString connectionString)
        {
            return connectionString;
        }

        protected virtual StorageProviders? OnExtractProviderType(IConnectionString connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString.Provider))
                return null;

            return (StorageProviders)Enum.Parse(typeof(StorageProviders), connectionString.Provider, true);
        }

        protected virtual BackgroundIndexing OnExtractBackgroundIndexing(IConnectionString connectionString)
        {
            return (string.IsNullOrWhiteSpace(ClientConnectionString.BackgroundIndexing))
                ? BackgroundIndexing.Off
                : (BackgroundIndexing)Enum.Parse(typeof(BackgroundIndexing), ClientConnectionString.BackgroundIndexing, true);
        }

        protected abstract string OnExtractDbName(IConnectionString connectionString);
    }
}