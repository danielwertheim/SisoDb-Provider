using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SisoDb.Serialization
{
    public class ParallelJsonBatchDeserializer : IBatchDeserializer
    {
        public IEnumerable<T> Deserialize<T>(IEnumerable<string> sourceData) where T : class
        {
            var q = new ConcurrentQueue<string>();

            var task = new Task(() =>
                                    {
                                        foreach (var json in sourceData)
                                        {
                                            q.Enqueue(json);
                                        }
                                    });
            task.Start();

            while (!task.IsCompleted)
            {
                string json;
                if (q.TryDequeue(out json))
                    yield return JsonSerialization.ToItemOrNull<T>(json);
            }

            Task.WaitAll(task);

            string json2;
            while (q.TryDequeue(out json2))
            {
                yield return JsonSerialization.ToItemOrNull<T>(json2);
            }
        }
    }
}