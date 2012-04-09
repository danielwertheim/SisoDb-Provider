using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;

namespace SisoDb.Querying
{
	public class SisoQueryable<T> : ISisoQueryable<T> where T : class
	{
	    protected readonly IQueryEngine QueryEngine;
		protected readonly IQueryBuilder<T> QueryBuilder;

        public SisoQueryable(IQueryBuilder<T> queryBuilder, IQueryEngine queryEngine)
		{
            Ensure.That(queryBuilder, "queryBuilder").IsNotNull();
            Ensure.That(queryEngine, "queryEngine").IsNotNull();

            QueryBuilder = queryBuilder;
            QueryEngine = queryEngine;
		}

        public virtual bool Any()
        {
            return QueryBuilder.IsEmpty 
                ? QueryEngine.Any<T>()
                : QueryEngine.Any<T>(QueryBuilder.Build());
        }

        public virtual bool Any(Expression<Func<T, bool>> expression)
        {
            Ensure.That(expression, "expression").IsNotNull();

            QueryBuilder.Clear();
            QueryBuilder.Where(expression);

            return QueryEngine.Any<T>(QueryBuilder.Build());
        }

        public virtual int Count()
        {
            return QueryBuilder.IsEmpty
                ? QueryEngine.Count<T>()
                : QueryEngine.Count<T>(QueryBuilder.Build());
        }

        public virtual int Count(Expression<Func<T, bool>> expression)
        {
            Ensure.That(expression, "expression").IsNotNull();

            QueryBuilder.Clear();
            QueryBuilder.Where(expression);

            return QueryEngine.Count<T>(QueryBuilder.Build());
        }

	    public virtual bool Exists(object id)
	    {
	        Ensure.That(id, "id").IsNotNull();

	        return QueryEngine.Exists<T>(id);
	    }

	    public virtual T First()
		{
			return ToEnumerable().First();
		}

		public virtual TResult FirstAs<TResult>() where TResult : class
		{
			return ToEnumerableOf<TResult>().First();
		}

		public virtual string FirstAsJson()
		{
			return ToEnumerableOfJson().First();
		}

		public virtual T FirstOrDefault()
		{
			return ToEnumerable().FirstOrDefault();
		}

		public virtual TResult FirstOrDefaultAs<TResult>() where TResult : class
		{
			return ToEnumerableOf<TResult>().FirstOrDefault();
		}

		public virtual string FirstOrDefaultAsJson()
		{
			return ToEnumerableOfJson().FirstOrDefault();
		}

		public virtual T Single()
		{
			return ToEnumerable().Single();
		}

		public virtual TResult SingleAs<TResult>() where TResult : class
		{
			return ToEnumerableOf<TResult>().Single();
		}

		public virtual string SingleAsJson()
		{
			return ToEnumerableOfJson().Single();
		}

		public virtual T SingleOrDefault()
		{
			return ToEnumerable().SingleOrDefault();
		}

		public virtual TResult SingleOrDefaultAs<TResult>() where TResult : class 
		{
			return ToEnumerableOf<TResult>().SingleOrDefault();
		}

		public virtual string SingleOrDefaultAsJson()
		{
			return ToEnumerableOfJson().SingleOrDefault();
		}

        public virtual T[] ToArray()
        {
            return ToEnumerable().ToArray();
        }

        public virtual TResult[] ToArrayOf<TResult>() where TResult : class
        {
            return ToEnumerableOf<TResult>().ToArray();
        }

        public virtual TResult[] ToArrayOf<TResult>(TResult template) where TResult : class
        {
            Ensure.That(template, "template").IsNotNull();

            return ToEnumerableOf(template).ToArray();
        }

        public virtual string[] ToArrayOfJson()
        {
            return ToEnumerableOfJson().ToArray();
        }

        public virtual IEnumerable<T> ToEnumerable()
        {
            return QueryEngine.Query<T>(QueryBuilder.Build());
        }

        public virtual IEnumerable<TResult> ToEnumerableOf<TResult>() where TResult : class
        {
            return QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build());
        }

        public virtual IEnumerable<TResult> ToEnumerableOf<TResult>(TResult template) where TResult : class
        {
            Ensure.That(template, "template").IsNotNull();

            return QueryEngine.QueryAsAnonymous<T, TResult>(QueryBuilder.Build(), template);
        }

        public virtual IEnumerable<string> ToEnumerableOfJson()
        {
            return QueryEngine.QueryAsJson<T>(QueryBuilder.Build());
        }

        public virtual IList<T> ToList()
        {
            return ToEnumerable().ToList();
        }

        public virtual IList<TResult> ToListOf<TResult>() where TResult : class
        {
            return ToEnumerableOf<TResult>().ToList();
        }

        public virtual IList<TResult> ToListOf<TResult>(TResult template) where TResult : class
        {
            Ensure.That(template, "template").IsNotNull();

            return ToEnumerableOf(template).ToList();
        }

        public virtual IList<string> ToListOfJson()
        {
            return ToEnumerableOfJson().ToList();
        }

        public virtual ISisoQueryable<T> Take(int numOfStructures)
		{
			QueryBuilder.Take(numOfStructures);
			
			return this;
		}

		public virtual ISisoQueryable<T> Page(int pageIndex, int pageSize)
		{
			QueryBuilder.Page(pageIndex, pageSize);

			return this;
		}

		public virtual ISisoQueryable<T> Include<TInclude>(params Expression<Func<T, object>>[] expressions) where TInclude : class
		{
            Ensure.That(expressions, "expressions").HasItems();

			QueryBuilder.Include<TInclude>(expressions);
			
			return this;
		}

		public virtual ISisoQueryable<T> Where(params Expression<Func<T, bool>>[] expressions)
		{
		    Ensure.That(expressions, "expressions").HasItems();

			QueryBuilder.Where(expressions);
			
			return this;
		}

		public virtual ISisoQueryable<T> OrderBy(params Expression<Func<T, object>>[] expressions)
		{
            Ensure.That(expressions, "expressions").HasItems();

			QueryBuilder.OrderBy(expressions);

			return this;
		}

		public virtual ISisoQueryable<T> OrderByDescending(params Expression<Func<T, object>>[] expressions)
		{
            Ensure.That(expressions, "expressions").HasItems();

			QueryBuilder.OrderByDescending(expressions);

			return this;
		}
	}
}