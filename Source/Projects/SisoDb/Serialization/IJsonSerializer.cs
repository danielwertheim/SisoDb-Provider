using System;
using System.Collections.Generic;

namespace SisoDb.Serialization
{
    public interface IJsonSerializer
    {
        bool DeserializeManyInParallel { get; set; }

        string Serialize<T>(T item) where T : class;

        IEnumerable<string> SerializeMany<T>(IEnumerable<T> items) where T : class;
        
        T Deserialize<T>(string json) where T : class;
        
        T DeserializeAnonymous<T>(string json) where T : class;

        T DeserializeAnonymous<T>(string json, T template) where T : class;

        object Deserialize(Type structureType, string json);

        IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class;

        IEnumerable<object> DeserializeMany(Type structureType, IEnumerable<string> sourceData);

        IEnumerable<T> DeserializeManyAnonymous<T>(IEnumerable<string> sourceData, T template) where T : class;
    }
}