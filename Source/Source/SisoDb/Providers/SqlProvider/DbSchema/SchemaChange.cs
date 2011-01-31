using System;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    [Serializable]
    internal class SchemaChange
    {
        internal SchemaChanges Change { get; private set; }

        internal string Name { get; private set; }

        internal string DbDataType { get; private set; }

        internal SchemaChange(SchemaChanges change, string name, string dbDataType)
        {
            Change = change;
            Name = name;
            DbDataType = dbDataType;
        }
    }
}