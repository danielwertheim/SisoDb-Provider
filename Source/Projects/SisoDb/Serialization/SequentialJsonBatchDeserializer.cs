using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace SisoDb.Serialization
{
    public class SequentialJsonBatchDeserializer : IJsonBatchDeserializer
    {
        private readonly IJsonSerializer _jsonSerializer;

        public SequentialJsonBatchDeserializer(IJsonSerializer jsonSerializer)
        {
            Ensure.That(() => jsonSerializer).IsNotNull();

            _jsonSerializer = jsonSerializer;
        }

        public IEnumerable<T> Deserialize<T>(IEnumerable<string> sourceData) where T : class
        {
            return sourceData.Select(json => _jsonSerializer.ToItemOrNull<T>(json));
        }
    }
}