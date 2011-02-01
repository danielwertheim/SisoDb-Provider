using System.Collections.Generic;

namespace SisoDb.Serialization
{
    public class SequentialJsonBatchDeserializer : IBatchDeserializer
    {
        public IEnumerable<T> Deserialize<T>(IEnumerable<string> sourceData) where T : class
        {
            foreach (var json in sourceData)
                yield return JsonSerialization.ToItemOrNull<T>(json);
        }
    }
}