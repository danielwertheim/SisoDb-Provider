using System;
using System.Linq.Expressions;

namespace SisoDb.Querying
{
	public interface IQueryBuilder<T> where T : class 
	{
		void Clear();
		IQuery Build();
		IQueryBuilder<T> Take(int numOfStructures);
		IQueryBuilder<T> Page(int pageIndex, int pageSize);
		IQueryBuilder<T> Include<TInclude>(params Expression<Func<T, object>>[] expressions) where TInclude : class;
		IQueryBuilder<T> Where(params Expression<Func<T, bool>>[] expressions);
		IQueryBuilder<T> OrderBy(params Expression<Func<T, object>>[] expressions);
		IQueryBuilder<T> OrderByDescending(params Expression<Func<T, object>>[] expressions);
	}
}