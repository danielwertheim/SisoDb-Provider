using System;
using SisoDb.Resources;

namespace SisoDb.Structures
{
    [Serializable]
    internal class StructureId : IStructureId, IEquatable<StructureId>
    {
        public IdTypes IdType { get; private set; }

        public Type DataType { get; private set; }

        public ValueType Value { get; private set; }

        private StructureId()
        {
        }

        public static StructureId NewIdentityId(int value)
        {
            var id = new StructureId
                         {
                             IdType = IdTypes.Identity, 
                             DataType = typeof (int)
                         };
            id.SetIdentityValue(value);

            return id;
        }

        public static StructureId NewGuidId(Guid value)
        {
            if (Guid.Empty.Equals(value))
                throw new ArgumentOutOfRangeException("value", ExceptionMessages.Id_GuidIsMissingValue);

            var id = new StructureId
                         {
                             IdType = IdTypes.Guid, 
                             DataType = typeof(Guid), 
                             Value = value
                         };

            return id;
        }

        public void SetIdentityValue(int value)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException("value", ExceptionMessages.Id_IdentityIsOutOfRange);

            Value = value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StructureId);
        }

        public bool Equals(StructureId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Value, Value);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}