using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Text;

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