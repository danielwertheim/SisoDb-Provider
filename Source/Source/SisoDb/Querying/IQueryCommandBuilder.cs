using System;
using System.Linq.Expressions;

namespace SisoDb.Querying
{
    public interface IQueryCommandBuilder<T> where T : class 
    {
        IQueryCommand Command { get; }

        IQueryCommandBuilder<T> Take(int numOfStructures);

        IQueryCommandBuilder<T> Where(Expression<Func<T, bool>> predicate);

        IQueryCommandBuilder<T> SortBy(params Expression<Func<T, object>>[] sortings);

        IQueryCommandBuilder<T> Include<TInclude>(params Expression<Func<T, object>>[] includes) where TInclude : class;
    }
}