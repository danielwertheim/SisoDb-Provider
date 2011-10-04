using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;

namespace SisoDb.Serialization
{
    public class ParallelJsonBatchDeserializer : IJsonBatchDeserializer
    {
        private readonly IJsonSerializer _jsonSerializer;

        public ParallelJsonBatchDeserializer(IJsonSerializer jsonSerializer)
        {
            Ensure.That(() => jsonSerializer).IsNotNull();

            _jsonSerializer = jsonSerializer;
        }

        public IEnumerable<T> Deserialize<T>(IEnumerable<string> sourceData) where T : class
        {
            using (var q = new BlockingCollection<string>())
            {
                var task = new Task(() =>
                                    {
                                        foreach (var json in sourceData)
                                            q.Add(json);
                                    });

                task.Start();

                while (!task.IsCompleted)
                {
                    string json;
                    if (q.TryTake(out json))
                        yield return _jsonSerializer.ToItemOrNull<T>(json);
                }

                Task.WaitAll(task);

                q.CompleteAdding();

                while (q.Count > 0)
                    yield return _jsonSerializer.ToItemOrNull<T>(q.Take());
            }
        }
    }
}