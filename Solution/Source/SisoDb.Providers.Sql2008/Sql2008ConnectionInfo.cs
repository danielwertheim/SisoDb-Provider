using System;
using System.Data.SqlClient;
using SisoDb.Core;
using SisoDb.Providers.Sql2008.Resources;
using SisoDb.Resources;

namespace SisoDb.Providers.Sql2008
{
    [Serializable]
    public class Sql2008ConnectionInfo : ISisoConnectionInfo
    {
        private readonly ISisoConnectionInfo _innerConnectionInfo;

        public string Name { get; private set; }

        public StorageProviders ProviderType
        {
            get { return _innerConnectionInfo.ProviderType; }
        }

        public IConnectionString ServerConnectionString { get; private set; }

        public IConnectionString ConnectionString
        {
            get { return _innerConnectionInfo.ConnectionString; }
        }

        public Sql2008ConnectionInfo(string connectionStringOrName) 
            : this(new SisoConnectionInfo(connectionStringOrName))
        {}

        public Sql2008ConnectionInfo(ISisoConnectionInfo connectionInfo)
        {
            _innerConnectionInfo = connectionInfo;

            EnsureValid();

            Initialize();
        }

        private void EnsureValid()
        {
            OnEnsureValid();
        }

        protected virtual void OnEnsureValid()
        {
            if (ProviderType != StorageProviders.Sql2008)
                throw new SisoDbException(Sql2008Exceptions.Sql2008Database_UnsupportedProviderSpecified
                    .Inject(ProviderType, StorageProviders.Sql2008));
        }

        private void Initialize()
        {
            var cnStringBuilder = new SqlConnectionStringBuilder(ConnectionString.PlainString);

            Name = cnStringBuilder.InitialCatalog;
            if (string.IsNullOrWhiteSpace(Name))
                throw new SisoDbException(ExceptionMessages.SqlDatabase_ConnectionInfo_MissingName);

            cnStringBuilder.InitialCatalog = string.Empty;

            ServerConnectionString = ConnectionString.ReplacePlain(cnStringBuilder.ConnectionString);
        }
    }
}