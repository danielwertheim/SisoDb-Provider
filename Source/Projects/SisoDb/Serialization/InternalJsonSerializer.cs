using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.EnsureThat;
using SisoDb.Structures.Schemas.Configuration;

namespace SisoDb.Serialization
{
    public class InternalJsonSerializer : ISisoDbSerializer
    {
        protected readonly Func<Type, IStructureTypeConfig> StructureTypeConfigResolver;

        public SerializerOptions Options { get; set; }

        protected JsonDateHandler DateHandler
        {
            get { return Options.DateSerializationMode.ToServiceStackValue(); }
        }

        public InternalJsonSerializer(Func<Type, IStructureTypeConfig> structureTypeConfigResolver)
        {
            StructureTypeConfigResolver = structureTypeConfigResolver;
            Options = new SerializerOptions();
        }

        protected virtual void OnConfigForSerialization<T>() where T : class
        {
            JsConfig.DateHandler = DateHandler;
            JsConfig.ExcludeTypeInfo = true;
            JsConfig<T>.ExcludeTypeInfo = true;
            JsConfig.IncludeNullValues = false;
        }

        protected virtual void OnConfigForDeserialization()
        {
            JsConfig.DateHandler = DateHandler;
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
                if (itemType == null)
                    itemType = item.GetType();

                yield return OnSerialize(item, itemType);
            }
        }

        protected virtual string OnSerialize(object item, Type itemType)
        {
            return item != null
                ? JsonSerializer.SerializeToString(item, itemType) 
                : string.Empty;
        }

        public virtual T Deserialize<T>(string json) where T : class
        {
            OnConfigForDeserialization();

            return OnDeserialize<T>(json);
        }

        public virtual object Deserialize(string json, Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            OnConfigForDeserialization();

            return OnDeserialize(json, structureType);
        }

        public virtual TTemplate DeserializeUsingTemplate<TTemplate>(string json) where TTemplate : class
        {
            var templateType = typeof (TTemplate);

            OnConfigForDeserialization();
            TypeConfig<TTemplate>.EnableAnonymousFieldSetters = true;

            return OnDeserialize<TTemplate>(json, templateType);
        }

        public virtual IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class
        {
            OnConfigForDeserialization();
            return sourceData.Select(OnDeserialize<T>);
        }

        public virtual IEnumerable<object> DeserializeMany(IEnumerable<string> sourceData, Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            OnConfigForDeserialization();
            return sourceData.Select(json => OnDeserialize(json, structureType));
        }

        public virtual IEnumerable<TTemplate> DeserializeManyUsingTemplate<TTemplate>(IEnumerable<string> sourceData, Type templateType) where TTemplate : class
        {
            Ensure.That(templateType, "templateType").IsNotNull();

            OnConfigForDeserialization();
            TypeConfig<TTemplate>.EnableAnonymousFieldSetters = true;
            return sourceData.Select(json => OnDeserialize<TTemplate>(json, templateType));
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
    }
}