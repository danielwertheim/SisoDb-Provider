using System.Collections.Generic;
using SisoDb.Core;

namespace SisoDb.Serialization
{
    public class SequentialJsonBatchDeserializer : IBatchDeserializer
    {
        public IJsonSerializer JsonSerializer { private get; set; }

        public SequentialJsonBatchDeserializer(IJsonSerializer jsonSerializer)
        {
            JsonSerializer = jsonSerializer.AssertNotNull("jsonSerializer");
        }

        public IEnumerable<T> Deserialize<T>(IEnumerable<string> sourceData) where T : class
        {
            foreach (var json in sourceData)
                yield return JsonSerializer.ToItemOrNull<T>(json);
        }
    }
}