using System.Collections.Generic;
using System.Linq;

namespace SisoDb.Core
{
    internal class ElementBatcher : IElementBatcher
    {
        public int MaxBatchSize { get; set; }

        internal ElementBatcher(int maxBatchSize)
        {
            MaxBatchSize = maxBatchSize;
        }

        public IEnumerable<T[]> Batch<T>(IEnumerable<T> elements) where T : class
        {
            var elementsCount = elements.Count();
            var batchSize = elementsCount > MaxBatchSize ? MaxBatchSize : elementsCount;
            var c = 0;
            while (true)
            {
                var batch = elements.Skip(c * batchSize).Take(batchSize);
                if (batch.Count() < 1)
                    yield break;

                yield return batch.ToArray();

                c++;
            }
        }
    }
}