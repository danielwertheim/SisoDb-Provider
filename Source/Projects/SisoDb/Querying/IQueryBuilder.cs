using System;
using System.Linq.Expressions;

namespace SisoDb.Querying
{
    public interface IQueryBuilder
    {
        bool IsEmpty { get; }
        void Clear();
        IQuery Build();
        IQueryBuilder Take(int numOfStructures);
        IQueryBuilder Page(int pageIndex, int pageSize);
        //TODO: Rem for v16.0.0 final
        //IQueryBuilder Include(Type includeType, params LambdaExpression[] expressions);
        IQueryBuilder Where(params LambdaExpression[] expressions);
        IQueryBuilder OrderBy(params LambdaExpression[] expressions);
        IQueryBuilder OrderByDescending(params LambdaExpression[] expressions);
    }

	public interface IQueryBuilder<T> where T : class
	{
	    bool IsEmpty { get; }
		void Clear();
		IQuery Build();
		IQueryBuilder<T> Take(int numOfStructures);
		IQueryBuilder<T> Page(int pageIndex, int pageSize);
        //TODO: Rem for v16.0.0 final
        //IQueryBuilder<T> Include<TInclude>(params Expression<Func<T, object>>[] expressions) where TInclude : class;
		IQueryBuilder<T> Where(params Expression<Func<T, bool>>[] expressions);
		IQueryBuilder<T> OrderBy(params Expression<Func<T, object>>[] expressions);
		IQueryBuilder<T> OrderByDescending(params Expression<Func<T, object>>[] expressions);
	}
}