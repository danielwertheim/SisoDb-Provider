using System;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    [Serializable]
    internal class SqlDbColumn
    {
        internal string Name { get; private set; }

        internal string DbDataType { get; private set; }

        internal SqlDbColumn(string name, string dbDataType)
        {
            Name = name;
            DbDataType = dbDataType;
        }
    }
}