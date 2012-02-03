using System;
using System.Data.SqlClient;
using NCore;
using SisoDb.Resources;

namespace SisoDb.Sql2012
{
    [Serializable]
    public class Sql2012ConnectionInfo : SisoConnectionInfo
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

        public Sql2012ConnectionInfo(string connectionStringOrName) 
            : this(GetConnectionString(connectionStringOrName)) 
        { }

        public Sql2012ConnectionInfo(IConnectionString connectionString) 
            : base(connectionString)
        {
            if (ProviderType != StorageProviders.Sql2012)
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_UnsupportedProviderSpecified.Inject(ProviderType, StorageProviders.Sql2012));

            InitializeDbNameAndServerConnectionString();
        }

        private void InitializeDbNameAndServerConnectionString()
        {
            var cnStringBuilder = new SqlConnectionStringBuilder(ConnectionString.PlainString);

            _dbName = cnStringBuilder.InitialCatalog;

            if (string.IsNullOrWhiteSpace(_dbName))
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_MissingName);

            cnStringBuilder.InitialCatalog = string.Empty;

            _serverConnectionString = ConnectionString.ReplacePlain(cnStringBuilder.ConnectionString);
        }
    }
}