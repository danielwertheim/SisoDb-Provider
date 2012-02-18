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

        public abstract StorageProviders ProviderType { get; }

        public BackgroundIndexing BackgroundIndexing { get; private set; }

        public IConnectionString ClientConnectionString { get; private set; }

        public IConnectionString ServerConnectionString { get; private set; }

        protected SisoConnectionInfo(IConnectionString connectionString)
        {
            Ensure.That(connectionString, "connectionString").IsNotNull();
            EnsureCorrectProvider(connectionString);
            
            ClientConnectionString = FormatConnectionString(connectionString);
            ServerConnectionString = FormatServerConnectionString(connectionString);
            BackgroundIndexing = ExtractBackgroundIndexing(ClientConnectionString);
            
            if (BackgroundIndexing != BackgroundIndexing.Off)
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_BackgroundIndexingNotSupported.Inject(ProviderType));
        }

        private void EnsureCorrectProvider(IConnectionString connectionString)
        {
            var providerType = ExtractProviderType(connectionString);

            if (providerType != ProviderType)
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_UnsupportedProviderSpecified.Inject(providerType, ProviderType));
        }

        private IConnectionString FormatConnectionString(IConnectionString connectionString)
        {
            return OnFormatConnectionString(connectionString);
        }

        private IConnectionString FormatServerConnectionString(IConnectionString connectionString)
        {
            return OnFormatServerConnectionString(connectionString);
        }

        private StorageProviders ExtractProviderType(IConnectionString connectionString)
        {
            return OnExtractProviderType(connectionString);
        }

        private BackgroundIndexing ExtractBackgroundIndexing(IConnectionString connectionString)
        {
            return OnExtractBackgroundIndexing(connectionString);
        }

        protected virtual IConnectionString OnFormatConnectionString(IConnectionString connectionString)
        {
            return connectionString;
        }

        protected virtual IConnectionString OnFormatServerConnectionString(IConnectionString connectionString)
        {
            return connectionString;
        }

        protected virtual StorageProviders OnExtractProviderType(IConnectionString connectionString)
        {
            return (StorageProviders)Enum.Parse(typeof(StorageProviders), connectionString.Provider, true);
        }

        protected virtual BackgroundIndexing OnExtractBackgroundIndexing(IConnectionString connectionString)
        {
            return (string.IsNullOrWhiteSpace(ClientConnectionString.BackgroundIndexing))
                ? BackgroundIndexing.Off 
                : (BackgroundIndexing)Enum.Parse(typeof(BackgroundIndexing), ClientConnectionString.BackgroundIndexing, true);
        }
    }
}