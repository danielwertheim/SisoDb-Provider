using System;
using System.Collections;
using System.Collections.Generic;
using SisoDb.Resources;

namespace SisoDb.Reflections
{
    public static class TypeExtensions
    {
        private static readonly Type EnumerableType = typeof(IEnumerable);
        private static readonly Type DictionaryType = typeof(IDictionary);
        private static readonly Type EnumType = typeof(Enum);

        private static readonly Type StringType = typeof(string);
        private static readonly Type DateTimeType = typeof(DateTime);
        private static readonly Type BoolType = typeof(bool);
        private static readonly Type GuidType = typeof(Guid);
        private static readonly Type CharType = typeof(char);

        private static readonly Type ByteType = typeof(byte);
        private static readonly Type ShortType = typeof(short);
        private static readonly Type IntType = typeof(int);
        private static readonly Type LongType = typeof(long);

        private static readonly Type SingleType = typeof(Single);
        private static readonly Type FloatType = typeof(float);
        private static readonly Type DecimalType = typeof(decimal);
        private static readonly Type DoubleType = typeof(double);

        private static readonly Type NullableType = typeof(Nullable<>);

        private static readonly Type NullableDateTimeType = typeof(DateTime?);
        private static readonly Type NullableGuidType = typeof(Guid?);
        private static readonly Type NullableBoolType = typeof(bool?);
        private static readonly Type NullableCharType = typeof(Char?);

        private static readonly Type NullableByteType = typeof(byte?);
        private static readonly Type NullableShortType = typeof(short?);
        private static readonly Type NullableIntType = typeof(int?);
        private static readonly Type NullableLongType = typeof(long?);

        private static readonly Type NullableSingleType = typeof(Single?);
        private static readonly Type NullableFloatType = typeof(float?);
        private static readonly Type NullableDecimalType = typeof(decimal?);
        private static readonly Type NullableDoubleType = typeof(double?);

        private static readonly HashSet<Type> ExtraPrimitiveTypes = new HashSet<Type> { typeof(string), typeof(Guid), typeof(DateTime), typeof(Decimal) };

        public static bool IsSimpleType(this Type type)
        {
            return (type.IsValueType || type.IsPrimitive) || type.IsEnum || ExtraPrimitiveTypes.Contains(type);
        }

        public static bool IsEnumerableType(this Type type)
        {
            return !type.IsSimpleType()
                && EnumerableType.IsAssignableFrom(type)
                && !DictionaryType.IsAssignableFrom(type)
                && (type.IsGenericType || type.HasElementType);
        }

        public static bool IsEnumerableBytesType(this Type type)
        {
            if (!IsEnumerableType(type))
                return false;

            var elementType = GetEnumerableElementType(type);

            return elementType.IsByteType() || elementType.IsNullableByteType();
        }

        public static Type GetEnumerableElementType(this Type type)
        {
            return type.IsGenericType ? ExtractGenericType(type) : type.GetElementType();
        }

        private static Type ExtractGenericType(Type type)
        {
            var generics = type.GetGenericArguments();

            if (generics.Length != 1)
                throw new SisoDbException(ExceptionMessages.TypeExtensions_ExtractGenericType);

            return generics[0];
        }

        public static bool IsStringType(this Type t)
        {
            return t == StringType;
        }

        public static bool IsDateTimeType(this Type t)
        {
            return t == DateTimeType;
        }

        public static bool IsBoolType(this Type t)
        {
            return t == BoolType;
        }

        public static bool IsDecimalType(this Type t)
        {
            return t == DecimalType;
        }

        public static bool IsSingleType(this Type t)
        {
            return t == SingleType;
        }

        public static bool IsFloatType(this Type t)
        {
            return t == FloatType;
        }

        public static bool IsDoubleType(this Type t)
        {
            return t == DoubleType;
        }

        public static bool IsLongType(this Type t)
        {
            return t == LongType;
        }

        public static bool IsGuidType(this Type t)
        {
            return t == GuidType;
        }

        public static bool IsIntType(this Type t)
        {
            return t == IntType;
        }

        public static bool IsByteType(this Type t)
        {
            return t == ByteType;
        }

        public static bool IsShortType(this Type t)
        {
            return t == ShortType;
        }

        public static bool IsCharType(this Type t)
        {
            return t == CharType;
        }

        public static bool IsNullableDateTimeType(this Type t)
        {
            return t == NullableDateTimeType;
        }

        public static bool IsNullableDecimalType(this Type t)
        {
            return t == NullableDecimalType;
        }

        public static bool IsNullableSingleType(this Type t)
        {
            return t == NullableSingleType;
        }

        public static bool IsNullableFloatType(this Type t)
        {
            return t == NullableFloatType;
        }

        public static bool IsNullableDoubleType(this Type t)
        {
            return t == NullableDoubleType;
        }

        public static bool IsNullableBoolType(this Type t)
        {
            return t == NullableBoolType;
        }

        public static bool IsNullableGuidType(this Type t)
        {
            return t == NullableGuidType;
        }

        public static bool IsNullableShortType(this Type t)
        {
            return t == NullableShortType;
        }

        public static bool IsNullableIntType(this Type t)
        {
            return t == NullableIntType;
        }

        public static bool IsNullableByteType(this Type t)
        {
            return t == NullableByteType;
        }

        public static bool IsNullableLongType(this Type t)
        {
            return t == NullableLongType;
        }

        public static bool IsNullableCharType(this Type t)
        {
            return t == NullableCharType;
        }

        public static bool IsEnumType(this Type t)
        {
            return (t.BaseType == EnumType) || t.IsEnum;
        }

        public static bool IsNullableEnumType(this Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == NullableType)
            {
                t = Nullable.GetUnderlyingType(t);
                return t.IsEnumType();
            }

            return false;
        }
    }
}