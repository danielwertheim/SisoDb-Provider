using System;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    [Serializable]
    public class SqlDbColumn
    {
        public string Name { get; private set; }

        public string DbDataType { get; private set; }

        public SqlDbColumn(string name, string dbDataType)
        {
            Name = name;
            DbDataType = dbDataType;
        }
    }
}