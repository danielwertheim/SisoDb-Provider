using System.Collections.Generic;

namespace SisoDb
{
    internal interface IElementBatcher
    {
        int MaxBatchSize { get; set; }

        IEnumerable<T[]> Batch<T>(IEnumerable<T> elements) where T : class;
    }
}