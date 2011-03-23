using System;
using SisoDb.Core;
using SisoDb.Resources;

namespace SisoDb.Structures
{
    [Serializable]
    public class StructureIndex : IStructureIndex, IEquatable<StructureIndex>
    {
        public IStructureId StructureId { get; private set; }

        public string Name { get; private set; }

        public object Value { get; private set; }

        public bool IsUnique { get; private set; }

        public StructureIndex(IStructureId structureId, string name, object value, bool isUnique)
        {
            structureId.AssertNotNull("structureId");
            name.AssertNotNullOrWhiteSpace("name");

            if((value != null) && !(value is string) && !(value is ValueType))
                throw new ArgumentException(ExceptionMessages.StructureIndex_ValueArgument_IncorrectType);

            StructureId = structureId;
            Name = name;
            Value = value;
            IsUnique = isUnique;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StructureIndex);
        }

        public bool Equals(StructureIndex other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.StructureId, StructureId) && Equals(other.Name, Name) && Equals(other.Value, Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (StructureId != null ? StructureId.GetHashCode() : 0);
                result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ (Value != null ? Value.GetHashCode() : 0);
                return result;
            }
        }
    }
}