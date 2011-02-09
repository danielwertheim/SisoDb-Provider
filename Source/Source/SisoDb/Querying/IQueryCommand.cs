using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb.Querying
{
    public interface IQueryCommand<T> where T : class
    {
        Expression<Func<T, bool>> Selector { get; }

        IEnumerable<LambdaExpression> Sortings { get; }

        bool HasSelector { get; }

        bool HasSortings { get; }
        
        IQueryCommand<T> Where(Expression<Func<T, bool>> selector);

        IQueryCommand<T> SortBy(params Expression<Func<T, dynamic>>[] sortings);
    }
}