using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using SisoDb.Dynamic;
using SisoDb.Structures.Schemas;

namespace SisoDb.Serialization
{
    public class ServiceStackJsonSerializer : IJsonSerializer
    {
        public string ToJsonOrEmptyString<T>(T item) where T : class
        {
            return ServiceStackJsonSerializer<T>.ToJsonOrEmptyString(item);
        }

        public T ToItemOrNull<T>(string json) where T : class
        {
            return ServiceStackJsonSerializer<T>.ToItemOrNull(json);
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

    public static class ServiceStackJsonSerializer<T> where T : class
    {
        static ServiceStackJsonSerializer()
        {
            TypeConfig<T>.Properties = TypeConfig<T>.Properties
                .Where(p => !TypeInfo.HasIdProperty(p.PropertyType)).ToArray();
        }

        public static string ToJsonOrEmptyString(T item)
        {
            if (item == null)
                return string.Empty;

            return JsonSerializer.SerializeToString(item);
        }

        public static T ToItemOrNull(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;
            return JsonSerializer.DeserializeFromString<T>(json);
        }
    }
}