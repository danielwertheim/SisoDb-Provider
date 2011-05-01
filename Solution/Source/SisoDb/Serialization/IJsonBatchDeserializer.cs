using System.Collections.Generic;

namespace SisoDb.Serialization
{
    public interface IJsonBatchDeserializer
    {
        IEnumerable<T> Deserialize<T>(IEnumerable<string> sourceData) where T : class;
    }
}