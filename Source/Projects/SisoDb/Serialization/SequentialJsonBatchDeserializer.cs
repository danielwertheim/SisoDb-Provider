using System.Collections.Generic;
using System.Linq;
using SisoDb.Core;

namespace SisoDb.Serialization
{
    public class SequentialJsonBatchDeserializer : IJsonBatchDeserializer
    {
        private readonly IJsonSerializer _jsonSerializer;

        public SequentialJsonBatchDeserializer(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer.AssertNotNull("jsonSerializer");
        }

        public IEnumerable<T> Deserialize<T>(IEnumerable<string> sourceData) where T : class
        {
            return sourceData.Select(json => _jsonSerializer.ToItemOrNull<T>(json));
        }
    }
}