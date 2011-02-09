using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb.Querying
{
    public interface IGetCommand<T> where T : class
    {
        IEnumerable<LambdaExpression> Sortings { get; }

        bool HasSortings { get; }

        IGetCommand<T> SortBy(params Expression<Func<T, object>>[] sortings);
    }
}