using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb.Querying
{
    public interface IQueryDefinition<T> where T : class
    {
        Expression<Func<T, bool>> Selector { get; }

        IEnumerable<LambdaExpression> Sortings { get; }

        bool HasSelector { get; }

        bool HasSortings { get; }
        
        IQueryDefinition<T> Where(Expression<Func<T, bool>> selector);

        IQueryDefinition<T> SortBy(params Expression<Func<T, dynamic>>[] sortings);
    }
}