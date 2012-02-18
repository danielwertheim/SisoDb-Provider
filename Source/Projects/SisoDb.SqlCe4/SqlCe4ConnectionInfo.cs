using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Web;
using SisoDb.Resources;

namespace SisoDb.SqlCe4
{
    [Serializable]
    public class SqlCe4ConnectionInfo : SisoConnectionInfo
    {
        private readonly string _serverPath;
        private readonly string _filePath;
        private readonly string _dbName;

        public override StorageProviders ProviderType
        {
            get { return StorageProviders.SqlCe4; }
        }

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

        public SqlCe4ConnectionInfo(string connectionStringOrName)
            : this(ConnectionString.Get(connectionStringOrName))
        { }

        public SqlCe4ConnectionInfo(IConnectionString connectionString)
            : base(connectionString)
        {
            _filePath = ExtractFilePath(ClientConnectionString);
            _serverPath = ExtractServerPath(FilePath);
            _dbName = ExtractDbName(FilePath);

            if (string.IsNullOrWhiteSpace(_dbName))
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_MissingName);
        }

        protected override IConnectionString OnFormatConnectionString(IConnectionString connectionString)
        {
            var cnString = base.OnFormatConnectionString(connectionString);
            var cnStringBuilder = new SqlCeConnectionStringBuilder(cnString.PlainString)
            {
                Enlist = false
            };

            return connectionString.ReplacePlain(cnStringBuilder.ConnectionString);
        }

        protected override IConnectionString OnFormatServerConnectionString(IConnectionString connectionString)
        {
            var cnString = base.OnFormatServerConnectionString(connectionString);
            var cnStringBuilder = new SqlCeConnectionStringBuilder(cnString.PlainString);
            cnStringBuilder.Enlist = false;

            return cnString.ReplacePlain(cnStringBuilder.ConnectionString);
        }

        private string ExtractFilePath(IConnectionString connectionString)
        {
            var cnStringBuilder = new SqlCeConnectionStringBuilder(connectionString.PlainString);

            var filePath = cnStringBuilder.DataSource;

            const string dataDirectorySwitch = "|DataDirectory|";
            if (filePath.StartsWith(dataDirectorySwitch, StringComparison.OrdinalIgnoreCase))
            {
                filePath = filePath.Substring(dataDirectorySwitch.Length);
                if (HttpContext.Current != null)
                    filePath = Path.Combine(HttpContext.Current.Server.MapPath("App_Data"), filePath);
            }

            return filePath;
        }

        private string ExtractDbName(string filePath)
        {
            return filePath.Contains(Path.DirectorySeparatorChar.ToString())
                ? Path.GetFileNameWithoutExtension(filePath)
                : filePath;
        }

        private string ExtractServerPath(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }
    }
}