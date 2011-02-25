using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using SisoDb.Dynamic;

namespace SisoDb.Serialization
{
    /// <summary>
    /// <![CDATA[http://www.servicestack.net, https://github.com/ServiceStack.ServiceStack.Text]]>
    /// </summary>
    public class ServiceStackJsonSerializer : IJsonSerializer
    {
        public string ToJsonOrEmptyString<T>(T item) where T : class
        {
            if (item == null)
                return string.Empty;
            return JsonSerializer.SerializeToString<T>(item);
        }

        public T ToItemOrNull<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;
            return JsonSerializer.DeserializeFromString<T>(json);
        }

        public IDictionary<string, object> ToTypedKeyValueOrNull(TypeDescriptor typeDescriptor, string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var kvRepresentation = ToItemOrNull<IDictionary<string, dynamic>>(json);
            if (kvRepresentation == null || kvRepresentation.Count < 1)
                return null;

            foreach (var key in kvRepresentation.Keys.ToArray())
            {
                var membername = key;
                var member = typeDescriptor.Get(membername);

                if (member == null)
                    continue;

                kvRepresentation[membername] = JsonSerializer.DeserializeFromString(kvRepresentation[membername], member.Type);
            }

            return kvRepresentation;
        }
    }
}