using System;

namespace SisoDb.Providers.Sql2008.DbSchema
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