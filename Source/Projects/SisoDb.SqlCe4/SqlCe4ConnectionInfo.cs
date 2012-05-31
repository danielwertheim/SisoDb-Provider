using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Web;
using SisoDb.Resources;
using SisoDb.SqlServer;

namespace SisoDb.SqlCe4
{
    [Serializable]
    public class SqlCe4ConnectionInfo : SqlServerConnectionInfo
    {
        private readonly string _serverPath;
        private readonly string _filePath;

        public string ServerPath
        {
            get { return _serverPath; }
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public SqlCe4ConnectionInfo(string connectionStringOrName)
            : this(ConnectionString.Get(connectionStringOrName))
        { }

        public SqlCe4ConnectionInfo(IConnectionString connectionString)
            : base(StorageProviders.SqlCe4, connectionString)
        {
            _filePath = ExtractFilePath(ClientConnectionString);
            _serverPath = Path.GetDirectoryName(FilePath);

            if (string.IsNullOrWhiteSpace(DbName))
                throw new SisoDbException(ExceptionMessages.ConnectionInfo_MissingName);
        }

        protected override IConnectionString OnFormatConnectionString(IConnectionString connectionString)
        {
            var cnString = base.OnFormatConnectionString(connectionString);
            var cnStringBuilder = new SqlCeConnectionStringBuilder(cnString.PlainString) { Enlist = false };

            return connectionString.ReplacePlain(cnStringBuilder.ConnectionString);
        }

        protected override IConnectionString OnFormatServerConnectionString(IConnectionString connectionString)
        {
            var cnString = base.OnFormatServerConnectionString(connectionString);
            var cnStringBuilder = new SqlCeConnectionStringBuilder(cnString.PlainString);
            cnStringBuilder.Enlist = false;

            return cnString.ReplacePlain(cnStringBuilder.ConnectionString);
        }

        protected override string OnExtractDbName(IConnectionString connectionString)
        {
            var filePath = ExtractFilePath(ClientConnectionString);

            return filePath.Contains(Path.DirectorySeparatorChar.ToString())
                ? Path.GetFileNameWithoutExtension(filePath)
                : filePath;
        }

        private static string ExtractFilePath(IConnectionString connectionString)
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
    }
}