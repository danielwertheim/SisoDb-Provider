using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace SisoDb.Serialization
{
    public class ServiceStackJsonSerializer : IJsonSerializer
    {
        public string Serialize<T>(T item) where T : class
        {
            return ServiceStackJsonSerializer<T>.Serialize(item);
        }

        public T Deserialize<T>(string json) where T : class
        {
            return ServiceStackJsonSerializer<T>.Deserialize(json);
        }

        public IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class
        {
            return DeserializeManyInParallel<T>(sourceData);
        }

        private IEnumerable<T> DeserializeManyInSequential<T>(IEnumerable<string> sourceData) where T : class
        {
            return sourceData.Select(Deserialize<T>);
        }

        private IEnumerable<T> DeserializeManyInParallel<T>(IEnumerable<string> sourceData) where T : class
        {
            using (var q = new BlockingCollection<string>())
            {
                var task = new Task(() =>
                {
                    foreach (var json in sourceData)
                        q.Add(json);
                });

                task.Start();

                while (!task.IsCompleted)
                {
                    string json;
                    if (q.TryTake(out json))
                        yield return Deserialize<T>(json);
                }

                Task.WaitAll(task);

                q.CompleteAdding();

                while (q.Count > 0)
                    yield return Deserialize<T>(q.Take());
            }
        }
    }

    public static class ServiceStackJsonSerializer<T> where T : class
    {
        static ServiceStackJsonSerializer()
        {

            TypeConfig<T>.Properties = ExcludePropertiesThatHoldStructures(TypeConfig<T>.Properties);
        }

        private static PropertyInfo[] ExcludePropertiesThatHoldStructures(IEnumerable<PropertyInfo> properties)
        {
            return properties.Where(p => 
                !SisoEnvironment.Resources.ResolveStructureSchemas().StructureTypeFactory.Reflecter.HasIdProperty(p.PropertyType)).ToArray();
        }

        public static string Serialize(T item)
        {
            return item == null 
                ? string.Empty 
                : JsonSerializer.SerializeToString(item);
        }

        public static T Deserialize(string json)
        {
            return string.IsNullOrWhiteSpace(json) 
                ? null 
                : JsonSerializer.DeserializeFromString<T>(json);
        }
    }
}