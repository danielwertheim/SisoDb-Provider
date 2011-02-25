using System.Collections.Generic;

namespace SisoDb.Serialization
{
    public interface IBatchDeserializer
    {
        IEnumerable<T> Deserialize<T>(IEnumerable<string> sourceData) where T : class;
    }
}