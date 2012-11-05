using System;
using SisoDb.SqlServer;

namespace SisoDb.Sql2005
{
    [Serializable]
    public class Sql2005ConnectionInfo : SqlServerConnectionInfo
    {
        public Sql2005ConnectionInfo(string connectionStringOrName)
            : base(StorageProviders.Sql2005, connectionStringOrName)
        { }
    }
}