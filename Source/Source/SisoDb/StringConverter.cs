using System;
using SisoDb.Resources;

namespace SisoDb
{
    internal static class StringConverter
    {
        internal static string AsString<T>(T value)
        {
            if (ReferenceEquals(null, value) || DBNull.Value.Equals(value))
                return null;

            if (value is string)
                return value as string;

            if (value is Guid)
                return value.ToString();

            if (value is DateTime)
            {
                var dt = Convert.ToDateTime(value);
                return GetAsString(dt);
            }

            if (value is IConvertible)
                return GetAsString((IConvertible)value);

            throw new NotSupportedException(ExceptionMessages.StringConverter_AsString_TypeOfValueIsNotSupported);
        }

        private static string GetAsString(IConvertible value)
        {
            return Convert.ToString(value, SisoDbEnvironment.FormatProvider);
        }

        private static string GetAsString(DateTime value)
        {
            return value.ToString(SisoDbEnvironment.DateTimePattern, SisoDbEnvironment.DateTimeFormatProvider);
        }
    }
}