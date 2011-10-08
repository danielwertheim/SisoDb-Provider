using System.Collections.Generic;

namespace SisoDb.Serialization
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T item) where T : class;

        T Deserialize<T>(string json) where T : class;

        IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class;
    }
}