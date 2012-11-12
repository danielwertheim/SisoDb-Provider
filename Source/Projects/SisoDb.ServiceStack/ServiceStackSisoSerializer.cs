using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using SisoDb.EnsureThat;
using SisoDb.Serialization;

namespace SisoDb.ServiceStack
{
    public class ServiceStackSisoSerializer : ISisoSerializer
    {
        protected const JsonDateHandler DefaultDateHandler = JsonDateHandler.ISO8601;

        protected virtual void OnConfigForSerialization<T>() where T : class
        {
            JsConfig.DateHandler = DefaultDateHandler;
            JsConfig.ExcludeTypeInfo = true;
            JsConfig<T>.ExcludeTypeInfo = true;
            JsConfig.IncludeTypeInfo = false;
            JsConfig<T>.IncludeTypeInfo = false;
            JsConfig.IncludeNullValues = false;
            JsConfig.TryToParsePrimitiveTypeValues = true;
        }

        protected virtual void OnConfigForDeserialization<T>() where T : class
        {
            OnConfigForDeserialization();
            TypeConfig<T>.EnableAnonymousFieldSetters = true;
        }

        protected virtual void OnConfigForDeserialization()
        {
            JsConfig.DateHandler = DefaultDateHandler;
        }

        public virtual string Serialize<T>(T item) where T : class
        {
            if (item == null)
                return string.Empty;

            OnConfigForSerialization<T>();

            return JsonSerializer.SerializeToString(item, item.GetType());
        }

        public virtual IEnumerable<string> SerializeMany<T>(IEnumerable<T> items) where T : class
        {
            OnConfigForSerialization<T>();

            Type itemType = null;
            foreach (var item in items)
            {
                if (item == null)
                {
                    yield return string.Empty;
                    continue;
                }

                if (itemType == null)
                    itemType = item.GetType();

                yield return JsonSerializer.SerializeToString(item, itemType);
            }
        }

        public virtual T Deserialize<T>(string json) where T : class
        {
            OnConfigForDeserialization<T>();

            return OnDeserialize<T>(json);
        }

        public virtual object Deserialize(string json, Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            OnConfigForDeserialization();

            return OnDeserialize(json, structureType);
        }

        public virtual IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class
        {
            OnConfigForDeserialization<T>();

            return sourceData.Select(OnDeserialize<T>);
        }

        public virtual IEnumerable<object> DeserializeMany(IEnumerable<string> sourceData, Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            OnConfigForDeserialization();

            return sourceData.Select(json => OnDeserialize(json, structureType));
        }

        protected virtual T OnDeserialize<T>(string json) where T : class
        {
            return string.IsNullOrEmpty(json)
                ? null
                : JsonSerializer.DeserializeFromString<T>(json);
        }

        protected virtual object OnDeserialize(string json, Type structureType)
        {
            return string.IsNullOrEmpty(json)
                ? null
                : JsonSerializer.DeserializeFromString(json, structureType);
        }
    }
}