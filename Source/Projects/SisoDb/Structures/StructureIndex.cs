using System;
using SisoDb.Core;
using SisoDb.Resources;

namespace SisoDb.Structures
{
    [Serializable]
    public class StructureIndex : IStructureIndex, IEquatable<IStructureIndex>
    {
        public ISisoId SisoId { get; private set; }

        public string Name { get; private set; }

        public object Value { get; private set; }

        public bool IsUnique { get; private set; }

        public StructureIndexType IndexType { get; private set; }

        public StructureIndex(ISisoId sisoId, string name, object value, StructureIndexType indexType = StructureIndexType.Normal)
        {
            sisoId.AssertNotNull("sisoId");
            name.AssertNotNullOrWhiteSpace("name");

            if((value != null) && !(value is string) && !(value is ValueType))
                throw new ArgumentException(ExceptionMessages.StructureIndex_ValueArgument_IncorrectType);

            SisoId = sisoId;
            Name = name;
            Value = value;
            IndexType = indexType;
            IsUnique = IndexType.IsUnique();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IStructureIndex);
        }

        public bool Equals(IStructureIndex other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.SisoId, SisoId) && Equals(other.Name, Name) && Equals(other.Value, Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (SisoId != null ? SisoId.GetHashCode() : 0);
                result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ (Value != null ? Value.GetHashCode() : 0);
                return result;
            }
        }
    }
}