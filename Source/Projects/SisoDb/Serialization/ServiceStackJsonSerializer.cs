using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace SisoDb.Serialization
{
    public class ServiceStackJsonSerializer : IJsonSerializer
    {
		public string Serialize<T>(T item) where T : class
        {
            if (item == null)
                return string.Empty;

			var itemType = item.GetType();

			ServiceStackTypeConfig<T>.Config(itemType);

			return JsonSerializer.SerializeToString(item, itemType);
        }

        public T Deserialize<T>(string json) where T : class
        {
            return string.IsNullOrWhiteSpace(json)
                ? null
                : JsonSerializer.DeserializeFromString<T>(json);
        }

        public IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class
        {
            return DeserializeManyInParallel<T>(sourceData);
        }

        //private IEnumerable<T> DeserializeManyInSequential<T>(IEnumerable<string> sourceData) where T : class
        //{
        //    return sourceData.Select(Deserialize<T>);
        //}

        private IEnumerable<T> DeserializeManyInParallel<T>(IEnumerable<string> sourceData) where T : class
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
                        yield return Deserialize<T>(json);
                }

                Task.WaitAll(task);

                q.CompleteAdding();

                while (q.Count > 0)
                    yield return Deserialize<T>(q.Take());
            }
        }
    }
}