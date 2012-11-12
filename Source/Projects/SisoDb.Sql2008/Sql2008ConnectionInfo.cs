using System;
using SisoDb.SqlServer;

namespace SisoDb.Sql2008
{
    [Serializable]
    public class Sql2008ConnectionInfo : SqlServerConnectionInfo
    {
        public Sql2008ConnectionInfo(string connectionStringOrName)
            : base(StorageProviders.Sql2008, connectionStringOrName)
        { }
    }
}