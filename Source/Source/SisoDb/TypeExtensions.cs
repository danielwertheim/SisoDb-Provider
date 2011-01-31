using System;
using System.Collections;
using System.Collections.Generic;
using SisoDb.Resources;

namespace SisoDb
{
    internal static class TypeExtensions
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

        internal static bool IsSimpleType(this Type type)
        {
            return (type.IsValueType || type.IsPrimitive) || type.IsEnum || ExtraPrimitiveTypes.Contains(type);
        }

        internal static bool IsEnumerableType(this Type type)
        {
            return !type.IsSimpleType()
                && EnumerableType.IsAssignableFrom(type)
                && !DictionaryType.IsAssignableFrom(type)
                && (type.IsGenericType || type.HasElementType);
        }

        internal static bool IsEnumerableBytesType(this Type type)
        {
            if (!IsEnumerableType(type))
                return false;

            var elementType = GetEnumerableElementType(type);

            return elementType.IsByteType() || elementType.IsNullableByteType();
        }

        internal static Type GetEnumerableElementType(this Type type)
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

        internal static bool IsStringType(this Type t)
        {
            return t == StringType;
        }

        internal static bool IsDateTimeType(this Type t)
        {
            return t == DateTimeType;
        }

        internal static bool IsBoolType(this Type t)
        {
            return t == BoolType;
        }

        internal static bool IsDecimalType(this Type t)
        {
            return t == DecimalType;
        }

        internal static bool IsSingleType(this Type t)
        {
            return t == SingleType;
        }

        internal static bool IsFloatType(this Type t)
        {
            return t == FloatType;
        }

        internal static bool IsDoubleType(this Type t)
        {
            return t == DoubleType;
        }

        internal static bool IsLongType(this Type t)
        {
            return t == LongType;
        }

        internal static bool IsGuidType(this Type t)
        {
            return t == GuidType;
        }

        internal static bool IsIntType(this Type t)
        {
            return t == IntType;
        }

        internal static bool IsByteType(this Type t)
        {
            return t == ByteType;
        }

        internal static bool IsShortType(this Type t)
        {
            return t == ShortType;
        }

        internal static bool IsCharType(this Type t)
        {
            return t == CharType;
        }

        internal static bool IsNullableDateTimeType(this Type t)
        {
            return t == NullableDateTimeType;
        }

        internal static bool IsNullableDecimalType(this Type t)
        {
            return t == NullableDecimalType;
        }

        internal static bool IsNullableSingleType(this Type t)
        {
            return t == NullableSingleType;
        }

        internal static bool IsNullableFloatType(this Type t)
        {
            return t == NullableFloatType;
        }

        internal static bool IsNullableDoubleType(this Type t)
        {
            return t == NullableDoubleType;
        }

        internal static bool IsNullableBoolType(this Type t)
        {
            return t == NullableBoolType;
        }

        internal static bool IsNullableGuidType(this Type t)
        {
            return t == NullableGuidType;
        }

        internal static bool IsNullableShortType(this Type t)
        {
            return t == NullableShortType;
        }

        internal static bool IsNullableIntType(this Type t)
        {
            return t == NullableIntType;
        }

        internal static bool IsNullableByteType(this Type t)
        {
            return t == NullableByteType;
        }

        internal static bool IsNullableLongType(this Type t)
        {
            return t == NullableLongType;
        }

        internal static bool IsNullableCharType(this Type t)
        {
            return t == NullableCharType;
        }

        internal static bool IsEnumType(this Type t)
        {
            return (t.BaseType == EnumType) || t.IsEnum;
        }

        internal static bool IsNullableEnumType(this Type t)
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