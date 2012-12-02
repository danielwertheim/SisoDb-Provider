using System;
using System.Linq.Expressions;

namespace SisoDb.Querying
{
    public interface IQueryBuilder
    {
        bool IsEmpty { get; }
        IQueryBuilder Clear();
        IQueryBuilder ClearSortings();
        IQueryBuilder ClearWheres();
        IQuery Build();
        IQueryBuilder First();
        IQueryBuilder Single();
        IQueryBuilder MakeCacheable();
        IQueryBuilder Skip(int numOfStructures);
        IQueryBuilder Take(int numOfStructures);
        IQueryBuilder Page(int pageIndex, int pageSize);
        IQueryBuilder Where(params LambdaExpression[] expressions);
        IQueryBuilder OrderBy(params LambdaExpression[] expressions);
        IQueryBuilder OrderByDescending(params LambdaExpression[] expressions);
    }

	public interface IQueryBuilder<T> where T : class
	{
	    bool IsEmpty { get; }
        IQueryBuilder<T> Clear();
        IQueryBuilder<T> ClearSortings();
        IQueryBuilder<T> ClearWheres();
        IQuery Build();
	    IQueryBuilder<T> First();
        IQueryBuilder<T> Single();
        IQueryBuilder<T> MakeCacheable();
        IQueryBuilder<T> Skip(int numOfStructures);
        IQueryBuilder<T> Take(int numOfStructures);
        IQueryBuilder<T> Page(int pageIndex, int pageSize);
		IQueryBuilder<T> Where(params Expression<Func<T, bool>>[] expressions);
		IQueryBuilder<T> OrderBy(params Expression<Func<T, object>>[] expressions);
		IQueryBuilder<T> OrderByDescending(params Expression<Func<T, object>>[] expressions);
	}
}