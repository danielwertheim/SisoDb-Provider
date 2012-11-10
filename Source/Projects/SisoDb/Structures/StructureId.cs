using System;
using SisoDb.NCore;
using SisoDb.NCore.Reflections;
using SisoDb.Resources;

namespace SisoDb.Structures
{
    [Serializable]
    public class StructureId : IStructureId
    {
        private static readonly Type StringType = typeof(string);
        private static readonly Type GuidType = typeof(Guid);
        private static readonly Type NullableGuidType = typeof(Guid?);
        private static readonly Type IntType = typeof(int);
        private static readonly Type NullableIntType = typeof(int?);
        private static readonly Type LongType = typeof(long);
        private static readonly Type NullableLongType = typeof(long?);

        private readonly StructureIdTypes _idType;
        private readonly object _value;
        private readonly Type _dataType;
        private readonly bool _isEmpty;

        public StructureIdTypes IdType
        {
            get { return _idType; }
        }

        public object Value
        {
            get { return _value; }
        }

        public Type DataType
        {
            get { return _dataType; }
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public static IStructureId ConvertFrom(object idValue)
        {
            var type = idValue.GetType();
            var idType = GetIdTypeFrom(type);

            if (idType == StructureIdTypes.String)
                return Create(idValue.ToString());

            if (idType == StructureIdTypes.Guid)
                return Create((Guid)idValue);

            if (idType == StructureIdTypes.Identity)
                return Create((int)idValue);

            if (idType == StructureIdTypes.BigIdentity)
                return Create((long)idValue);

            throw new SisoDbException(ExceptionMessages.StructureId_ConvertFrom.Inject(type));
        }

        public static IStructureId Create(string value)
        {
            return new StructureId(value, StringType);
        }

        public static IStructureId Create(Guid value)
        {
            return new StructureId(value, GuidType, StructureIdTypes.Guid, value == Guid.Empty);
        }

        public static IStructureId Create(Guid? value)
        {
            return new StructureId(value, NullableGuidType, StructureIdTypes.Guid, !value.HasValue || value == Guid.Empty);
        }

        public static IStructureId Create(int value)
        {
            return new StructureId(value, IntType, StructureIdTypes.Identity, value < 1);
        }

        public static IStructureId Create(int? value)
        {
            return new StructureId(value, NullableIntType, StructureIdTypes.Identity, !value.HasValue || value.Value < 1);
        }

        public static IStructureId Create(long value)
        {
            return new StructureId(value, LongType, StructureIdTypes.BigIdentity, value < 1);
        }

        public static IStructureId Create(long? value)
        {
            return new StructureId(value, NullableLongType, StructureIdTypes.BigIdentity, !value.HasValue || value.Value < 1);
        }

        public static IStructureId Create(object value, StructureIdTypes idType)
        {
            if (value is string)
                return Create(value as string, idType);

            var hasValue = value != null;

            switch (idType)
            {
                case StructureIdTypes.Identity:
                    return hasValue
                        ? Create((int)value)
                        : Create((int?)null);
                case StructureIdTypes.BigIdentity:
                    return hasValue
                        ? Create((long)value)
                        : Create((long?)null);
                case StructureIdTypes.Guid:
                    return hasValue
                        ? Create((Guid)value)
                        : Create((Guid?)null);
                case StructureIdTypes.String:
                    return hasValue
                        ? Create(value.ToString())
                        : Create(null as string);
            }

            throw new SisoDbException(ExceptionMessages.StructureId_CreateByIdType.Inject(
                hasValue ? value.GetType().Name : "null", 
                idType));
        }

        public static IStructureId Create(string value, StructureIdTypes idType)
        {
            var hasValue = !string.IsNullOrWhiteSpace(value);

            switch (idType)
            {
                case StructureIdTypes.Identity:
                    return hasValue
                        ? Create(int.Parse(value))
                        : Create((int?)null);
                case StructureIdTypes.BigIdentity:
                    return hasValue
                        ? Create(long.Parse(value))
                        : Create((long?)null);
                case StructureIdTypes.Guid:
                    return hasValue
                        ? Create(Guid.Parse(value))
                        : Create((Guid?)null);
                case StructureIdTypes.String:
                    return Create(value);
            }

            throw new SisoDbException(ExceptionMessages.StructureId_Create_FromString_WithSpecificId.Inject(value, idType.ToString()));
        }

        public static IStructureId GetSmallest(IStructureId x, IStructureId y)
        {
            return x.CompareTo(y) == -1
                ? x
                : y;
        }

        private StructureId(string value, Type dataType)
        {
            _value = value;
            _isEmpty = string.IsNullOrWhiteSpace(value);
            _dataType = dataType;
            _idType = StructureIdTypes.String;
        }

        private StructureId(ValueType value, Type dataType, StructureIdTypes idType, bool isEmpty)
        {
            _value = value;
            _isEmpty = isEmpty;
            _dataType = dataType;
            _idType = idType;
        }

        public static StructureIdTypes GetIdTypeFrom(Type type)
        {
            if (type.IsStringType())
                return StructureIdTypes.String;

            if (type.IsGuidType() || type.IsNullableGuidType())
                return StructureIdTypes.Guid;

            if (type.IsIntType() || type.IsNullableIntType())
                return StructureIdTypes.Identity;

            if (type.IsLongType() || type.IsNullableLongType())
                return StructureIdTypes.BigIdentity;

            throw new SisoDbException(ExceptionMessages.StructureId_InvalidType.Inject(type.Name));
        }

        public static bool IsValidDataType(Type type)
        {
            if (type.IsStringType())
                return true;

            if (type.IsGuidType() || type.IsNullableGuidType())
                return true;

            if (type.IsIntType() || type.IsNullableIntType())
                return true;

            if (type.IsLongType() || type.IsNullableLongType())
                return true;

            return false;
        }

        public int CompareTo(IStructureId other)
        {
            if (other.IdType != IdType)
                throw new SisoDbException(ExceptionMessages.StructureId_CompareTo_DifferentIdTypes);

            if (Equals(other))
                return 0;

            if (IdType == StructureIdTypes.Identity)
            {
                var x = (int?)Value;
                var y = (int?)other.Value;

                if (x.HasValue && y.HasValue)
                    return x.Value.CompareTo(y.Value);

                return x.HasValue ? -1 : 1;
            }

            if (IdType == StructureIdTypes.BigIdentity)
            {
                var x = (long?)Value;
                var y = (long?)other.Value;

                if (x.HasValue && y.HasValue)
                    return x.Value.CompareTo(y.Value);

                return x.HasValue ? -1 : 1;
            }

            if (IdType.IsGuid())
            {
                var x = (Guid?)Value;
                var y = (Guid?)other.Value;

                if (x.HasValue && y.HasValue)
                    return x.Value.CompareTo(y.Value);

                return x.HasValue ? -1 : 1;
            }

            return Sys.StringComparer.Compare(Value.ToString(), other.Value.ToString());
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IStructureId);
        }

        public bool Equals(IStructureId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (IdType == StructureIdTypes.String)
                return string.Equals(Value as string, other.Value as string, Sys.StringComparision);

            return Equals(other.Value, Value);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}