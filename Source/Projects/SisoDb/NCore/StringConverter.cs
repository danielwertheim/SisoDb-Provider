using System;
using NCore.Resources;

namespace NCore
{
    public class StringConverter : IStringConverter
    {
        private readonly IFormatProvider _formatProvider;
        private readonly IFormatProvider _dateTimeFormatProvider;
        private readonly string _dateTimePattern;

        public StringConverter(IFormatting formatting)
        {
            _formatProvider = formatting.FormatProvider;
            _dateTimeFormatProvider = formatting.DateTimeFormatProvider;
            _dateTimePattern = formatting.DateTimePattern;
        }

        public string AsString<T>(T value)
        {
            if (ReferenceEquals(null, value) || DBNull.Value.Equals(value))
                return null;

            if (value is string)
                return value as string;

            if (value is Guid)
                return value.ToString();

        	if (value is DateTime)
        		return GetAsString(Convert.ToDateTime(value));

        	if (value is IConvertible)
                return GetAsString((IConvertible)value);

            throw new NotSupportedException(ExceptionMessages.StringConverter_AsString_TypeOfValueIsNotSupported);
        }

        private string GetAsString(IConvertible value)
        {
            return Convert.ToString(value, _formatProvider);
        }

        private string GetAsString(DateTime value)
        {
            return value.ToString(_dateTimePattern, _dateTimeFormatProvider);
        }
    }
}