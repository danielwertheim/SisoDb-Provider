using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using SisoDb.Resources;

namespace SisoDb.Querying
{
	public class SisoReadOnceQueryable<T> : SisoQueryable<T> where T : class 
	{
		private readonly Func<IQueryEngine> _queryEngineFactory;

		protected override IQueryEngine QueryEngine
		{
			get { return _queryEngineFactory.Invoke(); }
		}

		public SisoReadOnceQueryable(IQueryBuilder<T> queryBuilder, Func<IQueryEngine> queryEngineFactory) : base(queryBuilder)
		{
			Ensure.That(queryEngineFactory, "queryEngineFactory").IsNotNull();

			_queryEngineFactory = queryEngineFactory;
		}

		public override int Count()
		{
			using (var qe = QueryEngine)
			{
				return qe.Count<T>(QueryBuilder.Build());
			}
		}

		public override int Count(System.Linq.Expressions.Expression<Func<T, bool>> expression)
		{
			using (var qe = QueryEngine)
			{
				return qe.Count<T>(QueryBuilder.Build());
			}
		}

		public override IEnumerable<T> ToEnumerable()
		{
			throw new SisoDbException(ExceptionMessages.ReadOnceQueryable_YieldingNotSupported);
		}

		public override IEnumerable<TResult> ToEnumerableOf<TResult>()
		{
			throw new SisoDbException(ExceptionMessages.ReadOnceQueryable_YieldingNotSupported);
		}

		public override IEnumerable<string> ToEnumerableOfJson()
		{
			throw new SisoDbException(ExceptionMessages.ReadOnceQueryable_YieldingNotSupported);
		}

		public override IList<T> ToList()
		{
			using (var qe = QueryEngine)
			{
				return qe.Query<T>(QueryBuilder.Build()).ToList();
			}
		}

		public override IList<TResult> ToListOf<TResult>()
		{
			using (var qe = QueryEngine)
			{
				return qe.QueryAs<T, TResult>(QueryBuilder.Build()).ToList();
			}
		}

		public override IList<string> ToListOfJson()
		{
			using (var qe = QueryEngine)
			{
				return qe.QueryAsJson<T>(QueryBuilder.Build()).ToList();
			}
		}

		public override T Single()
		{
			using (var qe = QueryEngine)
			{
				return qe.Query<T>(QueryBuilder.Build()).Single();
			}
		}

		public override TResult SingleAs<TResult>()
		{
			using (var qe = QueryEngine)
			{
				return qe.QueryAs<T, TResult>(QueryBuilder.Build()).Single();
			}
		}

		public override string SingleAsJson()
		{
			using (var qe = QueryEngine)
			{
				return qe.QueryAsJson<T>(QueryBuilder.Build()).Single();
			}
		}

		public override T SingleOrDefault()
		{
			using (var qe = QueryEngine)
			{
				return qe.Query<T>(QueryBuilder.Build()).SingleOrDefault();
			}
		}

		public override TResult SingleOrDefaultAs<TResult>()
		{
			using (var qe = QueryEngine)
			{
				return qe.QueryAs<T, TResult>(QueryBuilder.Build()).SingleOrDefault();
			}
		}

		public override string SingleOrDefaultAsJson()
		{
			using (var qe = QueryEngine)
			{
				return qe.QueryAsJson<T>(QueryBuilder.Build()).SingleOrDefault();
			}
		}
	}
}