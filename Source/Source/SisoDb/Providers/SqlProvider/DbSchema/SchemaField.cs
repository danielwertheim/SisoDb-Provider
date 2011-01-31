using System;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    [Serializable]
    internal class SchemaField
    {
        internal readonly int Ordinal;
        internal readonly string Name;

        internal SchemaField(int ordinal, string name)
        {
            Ordinal = ordinal;
            Name = name;
        }
    }
}