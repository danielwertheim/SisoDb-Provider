using System.Collections.Generic;
using System.Linq;
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
            return sourceData.Select(json => JsonSerializer.ToItemOrNull<T>(json));
        }
    }
}