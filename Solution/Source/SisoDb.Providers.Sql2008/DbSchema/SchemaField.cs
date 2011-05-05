using System;

namespace SisoDb.Providers.Sql2008.DbSchema
{
    [Serializable]
    public class SchemaField
    {
        public readonly int Ordinal;
        public readonly string Name;

        public SchemaField(int ordinal, string name)
        {
            Ordinal = ordinal;
            Name = name;
        }
    }
}