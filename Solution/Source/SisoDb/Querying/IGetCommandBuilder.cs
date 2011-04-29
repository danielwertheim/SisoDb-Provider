using System;
using System.Linq.Expressions;

namespace SisoDb.Querying
{
    public interface IGetCommandBuilder<T> where T : class
    {
        IGetCommand Command { get; }
        
        IGetCommandBuilder<T> SortBy(params Expression<Func<T, object>>[] sortings);

        IGetCommandBuilder<T> Include<TInclude>(params Expression<Func<T, object>>[] includes) where TInclude : class;
    }
}