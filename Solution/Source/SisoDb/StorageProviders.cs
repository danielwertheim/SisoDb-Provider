using System;

namespace SisoDb
{
    [Serializable]
    public enum StorageProviders
    {
        Sql2008 = 0,
        SqlAzure = 10,
        SqlCe4 = 20
    }
}