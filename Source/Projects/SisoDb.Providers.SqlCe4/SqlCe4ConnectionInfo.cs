using System;
using System.Data.SqlServerCe;
using System.IO;
using SisoDb.Core;
using SisoDb.SqlCe4.Resources;

namespace SisoDb.SqlCe4
{
    [Serializable]
    public class SqlCe4ConnectionInfo : ISisoConnectionInfo
    {
        private readonly ISisoConnectionInfo _innerConnectionInfo;

        public string Name { get; private set; }

        public string FilePath { get; private set; }

        public StorageProviders ProviderType
        {
            get { return _innerConnectionInfo.ProviderType; }
        }

        public IConnectionString ConnectionString
        {
            get { return _innerConnectionInfo.ConnectionString; }
        }

        public SqlCe4ConnectionInfo(string connectionStringOrName) 
            : this(new SisoConnectionInfo(connectionStringOrName))
        {}

        public SqlCe4ConnectionInfo(ISisoConnectionInfo connectionInfo)
        {
            _innerConnectionInfo = connectionInfo;

            EnsureValid();

            Initialize();
        }

        private void EnsureValid()
        {
            if (ProviderType != StorageProviders.SqlCe4)
                throw new SisoDbException(SqlCe4Exceptions.SqlCe4Database_UnsupportedProviderSpecified.Inject(ProviderType, StorageProviders.SqlCe4));
        }

        private void Initialize()
        {
            var cnStringBuilder = new SqlCeConnectionStringBuilder(ConnectionString.PlainString);

            FilePath = cnStringBuilder.DataSource;

            Name = FilePath.Contains(Path.DirectorySeparatorChar.ToString())
                       ? Path.GetFileNameWithoutExtension(FilePath)
                       : FilePath;
        }
    }
}