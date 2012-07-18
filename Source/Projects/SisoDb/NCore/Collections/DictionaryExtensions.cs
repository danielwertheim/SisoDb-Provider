using System.Collections.Generic;

namespace NCore.Collections
{
    public static class DictionaryExtensions
    {
        public static int GetInt(this IDictionary<string, string> kvs, string key, int defaultValue = 0)
        {
            var v = GetString(kvs, key);

            return v != null ? int.Parse(v) : defaultValue;
        }

        public static string GetString(this IDictionary<string, string> kvs, string key, string defaultValue = null)
        {
            return kvs.ContainsKey(key) ? kvs[key] : defaultValue;
        }
    }
}