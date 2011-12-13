using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;

namespace SisoDb.Querying
{
	public class SisoQueryable<T> : ISisoQueryable<T> where T : class
	{
		protected virtual IReadSession ReadSession { get; private set; }
		protected readonly IQueryBuilder<T> QueryBuilder;

		public SisoQueryable(IQueryBuilder<T> queryBuilder, IReadSession readSession)
			: this(queryBuilder)
		{
			Ensure.That(readSession, "readSession").IsNotNull();
			ReadSession = readSession;
		}

		protected SisoQueryable(IQueryBuilder<T> queryBuilder)
		{
			Ensure.That(queryBuilder, "queryBuilder").IsNotNull();
			QueryBuilder = queryBuilder;
		}

		public virtual IEnumerable<T> Yield()
		{
			return ReadSession.Query<T>(QueryBuilder.Build());
		}

		public virtual IEnumerable<TResult> YieldAs<TResult>() where TResult : class
		{
			return ReadSession.QueryAs<T, TResult>(QueryBuilder.Build());
		}

		public virtual IEnumerable<string> YieldAsJson()
		{
			return ReadSession.QueryAsJson<T>(QueryBuilder.Build());
		}

		public virtual IList<T> ToList()
		{
			return Yield().ToList();
		}

		public virtual IList<TResult> ToListOf<TResult>() where TResult : class
		{
			return YieldAs<TResult>().ToList();
		}

		public virtual IList<string> ToListOfJson()
		{
			return YieldAsJson().ToList();
		}

		public virtual T Single()
		{
			return Yield().Single();
		}

		public virtual TResult SingleAs<TResult>() where TResult : class
		{
			return YieldAs<TResult>().Single();
		}

		public virtual string SingleAsJson()
		{
			return YieldAsJson().Single();
		}

		public virtual T SingleOrDefault()
		{
			return Yield().SingleOrDefault();
		}

		public virtual TResult SingleOrDefaultAs<TResult>() where TResult : class 
		{
			return YieldAs<TResult>().SingleOrDefault();
		}

		public virtual string SingleOrDefaultAsJson()
		{
			return YieldAsJson().SingleOrDefault();
		}

		public virtual int Count()
		{
			QueryBuilder.Clear();

			return ReadSession.Count<T>(QueryBuilder.Build());
		}

		public virtual int Count(Expression<Func<T, bool>> expression)
		{
			QueryBuilder.Clear();

			return ReadSession.Count<T>(QueryBuilder.Where(expression).Build());
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