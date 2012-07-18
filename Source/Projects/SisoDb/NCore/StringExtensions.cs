using System;
using System.Collections.Generic;
using System.Linq;

namespace NCore
{
    public static class StringExtensions
    {
		public static bool EndsWithAny(this string value, params string[] match)
		{
			return match.Any(value.EndsWith);
		}

    	public static string ToStringOrNull(this object obj)
    	{
    	    return obj == null 
                ? null 
                : obj.ToString();
    	}

        public static string Inject(this string format, params object[] formattingArgs)
        {
            return string.Format(format, formattingArgs);
        }

        public static string Inject(this string format, params string[] formattingArgs)
        {
            return string.Format(format, formattingArgs);
        }

        public static string AppendWith(this string value, string appendWith)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return string.Concat(value, appendWith);
        }

        public static string PrependWith(this string value, string prependWith)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return string.Concat(prependWith, value);
        }

        public static IDictionary<string, string> ToKeyValues(this string keyValueString, char pairDelim, char keyValueDelim)
        {
            return keyValueString.Split(new[] { pairDelim }, StringSplitOptions.RemoveEmptyEntries)
                .Select(kv => kv.Split(new[] { keyValueDelim }, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(kv => kv[0], kv => kv[1]);
        }

        public static string ToJoinedString(this IEnumerable<string> strings, string separator, string itemFormat)
        {
            return string.Join(separator, strings.Select(s => string.Format(itemFormat, s)));
        }
    }
}