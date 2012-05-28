using System;
using SisoDb.SqlServer;

namespace SisoDb.Sql2012
{
    [Serializable]
    public class Sql2012ConnectionInfo : SqlServerConnectionInfo
    {
        public Sql2012ConnectionInfo(string connectionStringOrName)
            : this(ConnectionString.Get(connectionStringOrName))
        { }

        public Sql2012ConnectionInfo(IConnectionString connectionString)
            : base(StorageProviders.Sql2012, connectionString)
        { }
    }
}