using System;
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

        public virtual IEnumerable<string> SerializeMany<T>(IEnumerable<T> items) where T : class
        {
            return items.Select(Serialize);
        }

        public virtual T Deserialize<T>(string json) where T : class
        {
            JsConfig<Text>.DeSerializeFn = t => new Text(t);

            return OnDeserialize<T>(json);
        }

        public virtual object Deserialize(Type structureType, string json)
        {
            JsConfig<Text>.DeSerializeFn = t => new Text(t);

            return OnDeserialize(structureType, json);
        }

        public IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class
        {
            JsConfig<Text>.DeSerializeFn = t => new Text(t);

            return DeserializeManyInParallel
                    ? OnDeserializeManyInParallel(sourceData, OnDeserialize<T>)
                    : OnDeserializeManyInSequential(sourceData, OnDeserialize<T>);
        }

        public IEnumerable<T> DeserializeManyAnonymous<T>(IEnumerable<string> sourceData, T template) where T : class
        {
            JsConfig<Text>.DeSerializeFn = t => new Text(t);
            TypeConfig<T>.EnableAnonymousFieldSetters = true;
            var templateType = template.GetType();

            return DeserializeManyInParallel
                    ? OnDeserializeManyInParallel(sourceData, json => OnDeserializeAnonymous<T>(json, templateType))
                    : OnDeserializeManyInSequential(sourceData, json => OnDeserializeAnonymous<T>(json, templateType));
        }

        protected virtual T OnDeserialize<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonSerializer.DeserializeFromString<T>(json);
        }

        protected virtual object OnDeserialize(Type structureType, string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonSerializer.DeserializeFromString(json, structureType);
        }

        protected virtual T OnDeserializeAnonymous<T>(string json, Type templateType) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonSerializer.DeserializeFromString(json, templateType) as T;
        }

        protected virtual IEnumerable<T> OnDeserializeManyInSequential<T>(IEnumerable<string> sourceData, Func<string, T> deserializer) where T : class
        {
            return sourceData.Select(deserializer);
        }

        protected virtual IEnumerable<T> OnDeserializeManyInParallel<T>(IEnumerable<string> sourceData, Func<string, T> deserializer) where T : class
        {
            using (var q = new BlockingCollection<string>())
            {
                Task task = null;

                try
                {
                    task = Task.Factory.StartNew(() =>
                    {
                        foreach (var json in sourceData)
                            q.Add(json);

                        q.CompleteAdding();
                    });

                    foreach (var e in q.GetConsumingEnumerable())
                        yield return deserializer.Invoke(e);

                    Task.WaitAll(task);
                }
                finally
                {
                    if (task != null && task.Status == TaskStatus.RanToCompletion)
                        task.Dispose();

                    q.CompleteAdding();
                }
            }
        }
    }
}