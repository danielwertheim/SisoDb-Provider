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
        public string ToJsonOrEmptyString<T>(T item) where T : class
        {
            return ServiceStackJsonSerializer<T>.ToJsonOrEmptyString(item);
        }

        public T ToItemOrNull<T>(string json) where T : class
        {
            return ServiceStackJsonSerializer<T>.ToItemOrNull(json);
        }

        public IEnumerable<T> Deserialize<T>(IEnumerable<string> sourceData) where T : class
        {
            return ParallelDeserialize<T>(sourceData);
        }

        //private IEnumerable<T> SequentialDeserialize<T>(IEnumerable<string> sourceData) where T : class
        //{
        //    return sourceData.Select(ToItemOrNull<T>);
        //}

        private IEnumerable<T> ParallelDeserialize<T>(IEnumerable<string> sourceData) where T : class
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
                        yield return ToItemOrNull<T>(json);
                }

                Task.WaitAll(task);

                q.CompleteAdding();

                while (q.Count > 0)
                    yield return ToItemOrNull<T>(q.Take());
            }
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