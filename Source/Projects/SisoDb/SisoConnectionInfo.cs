using System;
using System.Data;
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

        /// <summary>
        /// Provides a hook to modify/wrap the connection created. For example to add profiling. 
        /// Defaults to returning the same connection unmoddified
        /// </summary>
        public Func<IDbConnection, IDbConnection> OnConnectionCreated
        {
            get { return onConnectionCreated ?? (con => con); }
            set { onConnectionCreated = value; }
        }
        private Func<IDbConnection, IDbConnection> onConnectionCreated;


        protected SisoConnectionInfo(IConnectionString connectionString)
        {
            Ensure.That(connectionString, "connectionString").IsNotNull();
            EnsureCorrectProviderIfItExists(connectionString);

            ClientConnectionString = FormatConnectionString(connectionString);
            ServerConnectionString = FormatServerConnectionString(connectionString);
            BackgroundIndexing = ExtractBackgroundIndexing(ClientConnectionString);

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
            return OnFormatConnectionString(connectionString);
        }

        private IConnectionString FormatServerConnectionString(IConnectionString connectionString)
        {
            return OnFormatServerConnectionString(connectionString);
        }

        private StorageProviders? ExtractProviderType(IConnectionString connectionString)
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
    }
}