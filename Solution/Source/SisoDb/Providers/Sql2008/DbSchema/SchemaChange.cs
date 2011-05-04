using System;

namespace SisoDb.Providers.Sql2008.DbSchema
{
    [Serializable]
    public class SchemaChange
    {
        public SchemaChanges Change { get; private set; }

        public string Name { get; private set; }

        public string DbDataType { get; private set; }

        public SchemaChange(SchemaChanges change, string name, string dbDataType)
        {
            Change = change;
            Name = name;
            DbDataType = dbDataType;
        }
    }
}