using System;
using System.Data.SqlClient;
using NCore;
using SisoDb.Resources;

namespace SisoDb.Sql2008
{
    [Serializable]
    public class Sql2008ConnectionInfo : SisoConnectionInfo
    {
        private string _dbName;
        private IConnectionString _serverConnectionString;

        public override string DbName
        {
            get { return _dbName; }
        }

        public override IConnectionString ServerConnectionString
        {
            get { return _serverConnectionString; }
        }

        public Sql2008ConnectionInfo(string connectionStringOrName)
            : this(ConnectionString.Get(connectionStringOrName)) 
        { }

        public Sql2008ConnectionInfo(IConnectionString connectionString) 
            : base(connectionString)
        {
            if (ProviderType != StorageProviders.Sql2008)
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_UnsupportedProviderSpecified.Inject(ProviderType, StorageProviders.Sql2008));

            InitializeDbNameAndServerConnectionString();
        }

        private void InitializeDbNameAndServerConnectionString()
        {
            var cnStringBuilder = new SqlConnectionStringBuilder(ClientConnectionString.PlainString);

            _dbName = cnStringBuilder.InitialCatalog;

            if (string.IsNullOrWhiteSpace(_dbName))
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_MissingName);

            cnStringBuilder.InitialCatalog = string.Empty;

            _serverConnectionString = ClientConnectionString.ReplacePlain(cnStringBuilder.ConnectionString);
        }
    }
}