using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace SisoDb.Serialization
{
    public class ServiceStackJsonSerializer : IJsonSerializer
    {
        public bool DeserializeManyInParallel { get; set; }

        public ServiceStackJsonSerializer()
        {
            DeserializeManyInParallel = true;
        }

        public virtual string Serialize<T>(T item) where T : class
        {
            if (item == null)
                return string.Empty;

            var itemType = item.GetType();

            ServiceStackTypeConfig<T>.Config(itemType);
            JsConfig<T>.ExcludeTypeInfo = true;
            JsConfig<Text>.SerializeFn = t => t.ToString();

            return JsonSerializer.SerializeToString(item, itemType);
        }

        public virtual T Deserialize<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            JsConfig<Text>.DeSerializeFn = t => new Text(t);

            return JsonSerializer.DeserializeFromString<T>(json);
        }

        public IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class
        {
            JsConfig<Text>.DeSerializeFn = t => new Text(t);

            return DeserializeManyInParallel
                    ? OnDeserializeManyInParallel<T>(sourceData)
                    : OnDeserializeManyInSequential<T>(sourceData);
        }

        protected virtual IEnumerable<T> OnDeserializeManyInSequential<T>(IEnumerable<string> sourceData) where T : class
        {
            return sourceData.Select(Deserialize<T>);
        }

        protected virtual IEnumerable<T> OnDeserializeManyInParallel<T>(IEnumerable<string> sourceData) where T : class
        {
            using (var q = new BlockingCollection<string>())
            {
                Task task = null;

                try
                {
                    task = new Task(() =>
                    {
                        foreach (var json in sourceData)
                            q.Add(json);

                        q.CompleteAdding();
                    });

                    task.Start();

                    foreach (var e in q.GetConsumingEnumerable())
                        yield return JsonSerializer.DeserializeFromString<T>(e);
                }
                finally
                {
                    if (task != null)
                    {
                        Task.WaitAll(task);
                        task.Dispose();
                    }

                    q.CompleteAdding();
                }
            }
        }
    }
}