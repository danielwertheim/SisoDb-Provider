using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb
{
	public interface ISisoQueryable<T> where T : class
	{
        bool Any();
        bool Any(Expression<Func<T, bool>> expression);
        int Count();
        int Count(Expression<Func<T, bool>> expression);
        bool Exists(object id);

        T First();
        TResult FirstAs<TResult>() where TResult : class;
        string FirstAsJson();
        T FirstOrDefault();
        TResult FirstOrDefaultAs<TResult>() where TResult : class;
        string FirstOrDefaultAsJson();
        
        T Single();
        TResult SingleAs<TResult>() where TResult : class;
        string SingleAsJson();
        T SingleOrDefault();
        TResult SingleOrDefaultAs<TResult>() where TResult : class;
        string SingleOrDefaultAsJson();

		T[] ToArray();
        TResult[] ToArrayOf<TResult>() where TResult : class;
        TResult[] ToArrayOf<TResult>(TResult template) where TResult : class;
        TResult[] ToArrayOf<TResult>(Expression<Func<T, TResult>> projection) where TResult : class;
        string[] ToArrayOfJson();

		IEnumerable<T> ToEnumerable();
        IEnumerable<TResult> ToEnumerableOf<TResult>() where TResult : class;
        IEnumerable<TResult> ToEnumerableOf<TResult>(TResult template) where TResult : class;
	    IEnumerable<TResult> ToEnumerableOf<TResult>(Expression<Func<T, TResult>> projection) where TResult : class;
        IEnumerable<string> ToEnumerableOfJson();

		IList<T> ToList();
        IList<TResult> ToListOf<TResult>() where TResult : class;
        IList<TResult> ToListOf<TResult>(TResult template) where TResult : class;
        IList<TResult> ToListOf<TResult>(Expression<Func<T, TResult>> projection) where TResult : class;
        IList<string> ToListOfJson();

		ISisoQueryable<T> Take(int numOfStructures);
        ISisoQueryable<T> Page(int pageIndex, int pageSize);
        ISisoQueryable<T> Include<TInclude>(params Expression<Func<T, object>>[] expression) where TInclude : class;
        ISisoQueryable<T> Where(params Expression<Func<T, bool>>[] expression);
        ISisoQueryable<T> OrderBy(params Expression<Func<T, object>>[] expressions);
        ISisoQueryable<T> OrderByDescending(params Expression<Func<T, object>>[] expressions);
	}
}