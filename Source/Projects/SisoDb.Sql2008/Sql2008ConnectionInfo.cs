using System;
using SisoDb.SqlServer;

namespace SisoDb.Sql2008
{
    [Serializable]
    public class Sql2008ConnectionInfo : SqlServerConnectionInfo
    {
        public Sql2008ConnectionInfo(string connectionStringOrName)
            : this(ConnectionString.Get(connectionStringOrName))
        { }

        public Sql2008ConnectionInfo(IConnectionString connectionString)
            : base(StorageProviders.Sql2008, connectionString)
        { }
    }
}