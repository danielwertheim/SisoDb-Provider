using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using SisoDb.Core;

namespace SisoDb.Serialization
{
    public class ParallelJsonBatchDeserializer : IBatchDeserializer
    {
        public IJsonSerializer JsonSerializer { private get; set; }

        public ParallelJsonBatchDeserializer(IJsonSerializer jsonSerializer)
        {
            JsonSerializer = jsonSerializer.AssertNotNull("jsonSerializer");
        }

        public IEnumerable<T> Deserialize<T>(IEnumerable<string> sourceData) where T : class
        {
            var q = new ConcurrentQueue<string>();

            var task = new Task(() =>
            {
                foreach (var json in sourceData)
                    q.Enqueue(json);
            });

            task.Start();

            while (!task.IsCompleted)
            {
                string json;
                if (q.TryDequeue(out json))
                    yield return JsonSerializer.ToItemOrNull<T>(json);
            }

            Task.WaitAll(task);

            string j2;
            while (q.TryDequeue(out j2))
                yield return JsonSerializer.ToItemOrNull<T>(j2);
        }
    }
}