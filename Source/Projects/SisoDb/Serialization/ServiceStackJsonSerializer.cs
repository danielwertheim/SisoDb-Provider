using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;

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
            JsConfig.ExcludeTypeInfo = true;
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

        public virtual object Deserialize(string json, Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            JsConfig<Text>.DeSerializeFn = t => new Text(t);

            return OnDeserialize(json, structureType);
        }

        public virtual TTemplate DeserializeUsingTemplate<TTemplate>(string json) where TTemplate : class
        {
            return DeserializeUsingTemplate<TTemplate>(json, typeof(TTemplate));
        }

        public virtual TTemplate DeserializeUsingTemplate<TTemplate>(string json, Type templateType) where TTemplate : class
        {
            Ensure.That(templateType, "templateType").IsNotNull();

            JsConfig<Text>.DeSerializeFn = t => new Text(t);
            TypeConfig<TTemplate>.EnableAnonymousFieldSetters = true;

            return OnDeserialize<TTemplate>(json, templateType);
        }

        public virtual IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class
        {
            JsConfig<Text>.DeSerializeFn = t => new Text(t);

            return DeserializeManyInParallel
                    ? OnDeserializeManyInParallel(sourceData, OnDeserialize<T>)
                    : OnDeserializeManyInSequential(sourceData, OnDeserialize<T>);
        }

        public virtual IEnumerable<object> DeserializeMany(IEnumerable<string> sourceData, Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            JsConfig<Text>.DeSerializeFn = t => new Text(t);

            return DeserializeManyInParallel
                    ? OnDeserializeManyInParallel(sourceData, json => OnDeserialize(json, structureType))
                    : OnDeserializeManyInSequential(sourceData, json => OnDeserialize(json, structureType));
        }

        public virtual IEnumerable<TTemplate> DeserializeManyUsingTemplate<TTemplate>(IEnumerable<string> sourceData, TTemplate template) where TTemplate : class
        {
            Ensure.That(template, "template").IsNotNull();

            return DeserializeManyUsingTemplate<TTemplate>(sourceData, template.GetType());
        }

        public virtual IEnumerable<TTemplate> DeserializeManyUsingTemplate<TTemplate>(IEnumerable<string> sourceData, Type templateType) where TTemplate : class
        {
            Ensure.That(templateType, "templateType").IsNotNull();

            JsConfig<Text>.DeSerializeFn = t => new Text(t);
            TypeConfig<TTemplate>.EnableAnonymousFieldSetters = true;

            return DeserializeManyInParallel
                    ? OnDeserializeManyInParallel(sourceData, json => OnDeserialize<TTemplate>(json, templateType))
                    : OnDeserializeManyInSequential(sourceData, json => OnDeserialize<TTemplate>(json, templateType));
        }

        protected virtual T OnDeserialize<T>(string json) where T : class
        {
            return string.IsNullOrWhiteSpace(json) 
                ? null 
                : JsonSerializer.DeserializeFromString<T>(json);
        }

        protected virtual object OnDeserialize(string json, Type structureType)
        {
            return string.IsNullOrWhiteSpace(json) 
                ? null 
                : JsonSerializer.DeserializeFromString(json, structureType);
        }

        protected virtual T OnDeserialize<T>(string json, Type templateType) where T : class
        {
            return string.IsNullOrWhiteSpace(json)
                ? null
                : JsonSerializer.DeserializeFromString(json, templateType) as T;
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