using System;
using SisoDb.Resources;

namespace SisoDb.Structures
{
    [Serializable]
    public class SisoId : ISisoId, IEquatable<SisoId>
    {
        public IdTypes IdType { get; private set; }

        public Type DataType { get; private set; }

        public ValueType Value { get; private set; }

        private SisoId()
        {
        }

        public static SisoId NewIdentityId(int value)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException("value", ExceptionMessages.Id_IdentityIsOutOfRange);

            var id = new SisoId
                         {
                             IdType = IdTypes.Identity, 
                             DataType = typeof (int),
                             Value = value
                         };

            return id;
        }

        public static SisoId NewGuidId(Guid value)
        {
            if (Guid.Empty.Equals(value))
                throw new ArgumentOutOfRangeException("value", ExceptionMessages.Id_GuidIsMissingValue);

            var id = new SisoId
                         {
                             IdType = IdTypes.Guid, 
                             DataType = typeof(Guid), 
                             Value = value
                         };

            return id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SisoId);
        }

        public bool Equals(SisoId other)
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