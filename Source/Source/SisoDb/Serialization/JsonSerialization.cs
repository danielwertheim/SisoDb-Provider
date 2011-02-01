using ServiceStack.Text;

namespace SisoDb.Serialization
{
    /// <summary>
    /// <![CDATA[http://www.servicestack.net, https://github.com/mythz/ServiceStack/, https://github.com/mythz/ServiceStack.Text]]>
    /// </summary>
    internal static class JsonSerialization
    {
        internal static string ToJsonOrEmptyString<T>(T item) where T : class
        {
            if (item == null)
                return string.Empty;
            return JsonSerializer.SerializeToString<T>(item);
        }

        internal static T ToItemOrNull<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;
            return JsonSerializer.DeserializeFromString<T>(json);
        }
    }
}