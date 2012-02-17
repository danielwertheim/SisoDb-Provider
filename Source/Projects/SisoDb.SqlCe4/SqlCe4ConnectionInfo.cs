using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Web;
using NCore;
using SisoDb.Resources;

namespace SisoDb.SqlCe4
{
    [Serializable]
    public class SqlCe4ConnectionInfo : SisoConnectionInfo
    {
        private readonly string _serverPath;
        private readonly string _filePath;
        private readonly string _dbName;
        private readonly IConnectionString _serverConnectionString;

        public string ServerPath
        {
            get { return _serverPath; }
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public override string DbName
        {
            get { return _dbName; }
        }

        public override IConnectionString ServerConnectionString
        {
            get { return _serverConnectionString; }
        }

        public SqlCe4ConnectionInfo(string connectionStringOrName)
            : this(ConnectionString.Get(connectionStringOrName))
        { }

        public SqlCe4ConnectionInfo(IConnectionString connectionString)
            : base(connectionString)
        {
            if (ProviderType != StorageProviders.SqlCe4)
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_UnsupportedProviderSpecified.Inject(ProviderType, StorageProviders.SqlCe4));

            if(BackgroundIndexing != BackgroundIndexing.Off)
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_ParallelInsertsNotSupported.Inject(ProviderType));

            var cnStringBuilder = new SqlCeConnectionStringBuilder(ClientConnectionString.PlainString)
            {
                Enlist = false
            };

            _filePath = cnStringBuilder.DataSource;

            const string dataDirectorySwitch = "|DataDirectory|";
            if (_filePath.StartsWith(dataDirectorySwitch, StringComparison.OrdinalIgnoreCase))
            {
                _filePath = _filePath.Substring(dataDirectorySwitch.Length);
                if (HttpContext.Current != null)
                    _filePath = Path.Combine(HttpContext.Current.Server.MapPath("App_Data"), _filePath);
            }

            _dbName = _filePath.Contains(Path.DirectorySeparatorChar.ToString())
                ? Path.GetFileNameWithoutExtension(_filePath)
                : _filePath;

            if (string.IsNullOrWhiteSpace(_dbName))
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_MissingName);

            _serverPath = Path.GetDirectoryName(_filePath);

            _serverConnectionString = ClientConnectionString.ReplacePlain(cnStringBuilder.ConnectionString);
        }
    }
}