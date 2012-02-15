using System.Collections.Generic;

namespace SisoDb.Serialization
{
    public interface IJsonSerializer
    {
        bool DeserializeManyInParallel { get; set; }

        string Serialize<T>(T item) where T : class;

        IEnumerable<string> SerializeMany<T>(IEnumerable<T> items) where T : class;
        
        T Deserialize<T>(string json) where T : class;

        IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class;

        IEnumerable<T> DeserializeManyAnonymous<T>(IEnumerable<string> sourceData, T template) where T : class;
    }
}