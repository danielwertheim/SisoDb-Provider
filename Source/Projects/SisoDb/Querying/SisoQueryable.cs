using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;

namespace SisoDb.Querying
{
	public class SisoQueryable<T> : ISisoQueryable<T> where T : class
	{
		protected virtual ISession Session { get; private set; }
		protected readonly IQueryBuilder<T> QueryBuilder;

		public SisoQueryable(IQueryBuilder<T> queryBuilder, ISession session)
			: this(queryBuilder)
		{
			Ensure.That(session, "Session").IsNotNull();
			Session = session;
		}

		protected SisoQueryable(IQueryBuilder<T> queryBuilder)
		{
			Ensure.That(queryBuilder, "queryBuilder").IsNotNull();
			QueryBuilder = queryBuilder;
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
			return Session.QueryEngine.Query<T>(QueryBuilder.Build());
		}

		public virtual IEnumerable<TResult> ToEnumerableOf<TResult>() where TResult : class
		{
			return Session.QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build());
		}

        public virtual IEnumerable<TResult> ToEnumerableOf<TResult>(TResult template) where TResult : class
        {
            Ensure.That(template, "template").IsNotNull();

            return Session.QueryEngine.QueryAsAnonymous<T, TResult>(QueryBuilder.Build(), template);
        }

		public virtual IEnumerable<string> ToEnumerableOfJson()
		{
			return Session.QueryEngine.QueryAsJson<T>(QueryBuilder.Build());
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

		public virtual int Count()
		{
			return Session.QueryEngine.Count<T>(QueryBuilder.Build());
		}

		public virtual int Count(Expression<Func<T, bool>> expression)
		{
			QueryBuilder.Clear();

			return Session.QueryEngine.Count<T>(QueryBuilder.Where(expression).Build());
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
			QueryBuilder.Include<TInclude>(expressions);
			
			return this;
		}

		public virtual ISisoQueryable<T> Where(params Expression<Func<T, bool>>[] expressions)
		{
			QueryBuilder.Where(expressions);
			
			return this;
		}

		public virtual ISisoQueryable<T> OrderBy(params Expression<Func<T, object>>[] expressions)
		{
			QueryBuilder.OrderBy(expressions);

			return this;
		}

		public virtual ISisoQueryable<T> OrderByDescending(params Expression<Func<T, object>>[] expressions)
		{
			QueryBuilder.OrderByDescending(expressions);

			return this;
		}
	}
}