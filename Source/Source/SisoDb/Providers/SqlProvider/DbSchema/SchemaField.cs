using System;

namespace SisoDb.Providers.SqlProvider.DbSchema
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