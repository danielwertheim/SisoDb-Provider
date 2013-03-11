using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Web;
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
            : base(StorageProviders.SqlCe4, connectionStringOrName)
        {
            _filePath = ExtractFilePath(ClientConnectionString);
            _serverPath = Path.GetDirectoryName(FilePath);
        }

        protected override string OnFormatClientConnectionString(string connectionString)
        {
            var cnStringBuilder = new SqlCeConnectionStringBuilder(connectionString) { Enlist = false };

            return cnStringBuilder.ConnectionString;
        }

        protected override string OnFormatServerConnectionString(string connectionString)
        {
            var cnStringBuilder = new SqlCeConnectionStringBuilder(connectionString) { Enlist = false };

            return cnStringBuilder.ConnectionString;
        }

        protected override string OnExtractDbName(string connectionString)
        {
            var filePath = ExtractFilePath(ClientConnectionString);

            return Path.GetFileNameWithoutExtension(filePath);
        }

        protected string ExtractFilePath(string connectionString)
        {
            var cnStringBuilder = new SqlCeConnectionStringBuilder(connectionString);

            var filePath = cnStringBuilder.DataSource;

            const string dataDirectorySwitch = "|DataDirectory|";
            if (filePath.StartsWith(dataDirectorySwitch, Sys.StringComparision))
            {
                filePath = filePath.Substring(dataDirectorySwitch.Length);
                if (HttpContext.Current != null)
                    filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), filePath);
            }

            return filePath;
        }
    }
}