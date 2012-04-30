using System;

namespace SisoDb.DbSchema
{
    [Serializable]
    public class SchemaField : IEquatable<SchemaField>
    {
        public readonly int Ordinal;
        public readonly string Name;

        public SchemaField(int ordinal, string name)
        {
            Ordinal = ordinal;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SchemaField);
        }

        public bool Equals(SchemaField other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}