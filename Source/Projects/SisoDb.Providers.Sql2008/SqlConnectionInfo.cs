using System;
using System.Data.SqlClient;
using NCore;
using SisoDb.Core;
using SisoDb.Resources;
using SisoDb.Sql2008.Resources;

namespace SisoDb.Sql2008
{
    [Serializable]
    public class SqlConnectionInfo : ISisoConnectionInfo
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

        public SqlConnectionInfo(string connectionStringOrName) 
            : this(new SisoConnectionInfo(connectionStringOrName))
        {}

        public SqlConnectionInfo(ISisoConnectionInfo connectionInfo)
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
                throw new SisoDbException(Sql2008Exceptions.SqlDatabase_UnsupportedProviderSpecified
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