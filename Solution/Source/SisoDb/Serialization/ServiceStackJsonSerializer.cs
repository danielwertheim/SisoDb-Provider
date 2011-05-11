using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Text;
using SisoDb.Dynamic;

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

    internal static class ServiceStackJsonSerializer<T> where T : class
    {
        static ServiceStackJsonSerializer()
        {
            TypeConfig<T>.Properties = ExcludePropertiesThatHoldStructures(TypeConfig<T>.Properties);
        }

        private static PropertyInfo[] ExcludePropertiesThatHoldStructures(IEnumerable<PropertyInfo> properties)
        {
            return properties.Where(p => 
                !SisoEnvironment.Resources.ResolveStructureTypeReflecter().HasIdProperty(p.PropertyType)).ToArray();
        }

        internal static string ToJsonOrEmptyString(T item)
        {
            return item == null 
                ? string.Empty 
                : JsonSerializer.SerializeToString(item);
        }

        internal static T ToItemOrNull(string json)
        {
            return string.IsNullOrWhiteSpace(json) 
                ? null 
                : JsonSerializer.DeserializeFromString<T>(json);
        }
    }
}