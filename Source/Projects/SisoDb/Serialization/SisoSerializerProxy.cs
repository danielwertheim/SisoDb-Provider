using System;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb.Serialization
{
    /// <summary>
    /// Extend this class and hook it in on your <see cref="ISisoDatabase"/>.
    /// It alows you to intercept serialized and deserialized values.
    /// </summary>
    public abstract class SisoSerializerProxy : ISisoSerializer 
    {
        protected readonly ISisoSerializer InnerSerializer;
        
        protected SisoSerializerProxy(ISisoSerializer serializer)
        {
            InnerSerializer = serializer;
        }

        protected abstract string OnSerialized(string data);

        protected abstract string OnDeserializing(string data);

        public virtual string Serialize<T>(T item) where T : class
        {
            return OnSerialized(InnerSerializer.Serialize(item));
        }

        public virtual IEnumerable<string> SerializeMany<T>(IEnumerable<T> items) where T : class
        {
            foreach (var data in InnerSerializer.SerializeMany(items))
                yield return OnSerialized(data);
        }

        public virtual T Deserialize<T>(string json) where T : class
        {
            return InnerSerializer.Deserialize<T>(OnDeserializing(json));
        }

        public virtual object Deserialize(string json, Type structureType)
        {
            return InnerSerializer.Deserialize(OnDeserializing(json), structureType);
        }

        public virtual IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class
        {
            return InnerSerializer.DeserializeMany<T>(sourceData.Select(OnDeserializing));
        }

        public virtual IEnumerable<object> DeserializeMany(IEnumerable<string> sourceData, Type structureType)
        {
            return InnerSerializer.DeserializeMany(sourceData.Select(OnDeserializing), structureType);
        }
    }
}