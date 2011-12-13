using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using SisoDb.Resources;

namespace SisoDb.Querying
{
	public class SisoReadOnceQueryable<T> : SisoQueryable<T> where T : class 
	{
		private readonly Func<IReadSession> _readSessionFactory;

		protected override IReadSession ReadSession
		{
			get { return _readSessionFactory.Invoke(); }
		}

		public SisoReadOnceQueryable(IQueryBuilder<T> queryBuilder, Func<IReadSession> readSessionFactory) : base(queryBuilder)
		{
			Ensure.That(readSessionFactory, "readSessionFactory").IsNotNull();

			_readSessionFactory = readSessionFactory;
		}

		public override int Count()
		{
			using (var readSession = ReadSession)
			{
				return readSession.Count<T>(QueryBuilder.Build());
			}
		}

		public override int Count(System.Linq.Expressions.Expression<Func<T, bool>> expression)
		{
			using (var readSession = ReadSession)
			{
				return readSession.Count<T>(QueryBuilder.Build());
			}
		}

		public override IEnumerable<T> Yield()
		{
			throw new SisoDbException(ExceptionMessages.ReadOnceQueryable_YieldingNotSupported);
		}

		public override IEnumerable<TResult> YieldAs<TResult>()
		{
			throw new SisoDbException(ExceptionMessages.ReadOnceQueryable_YieldingNotSupported);
		}

		public override IEnumerable<string> YieldAsJson()
		{
			throw new SisoDbException(ExceptionMessages.ReadOnceQueryable_YieldingNotSupported);
		}

		public override IList<T> ToList()
		{
			using (var readSession = ReadSession)
			{
				return readSession.Query<T>(QueryBuilder.Build()).ToList();
			}
		}

		public override IList<TResult> ToListOf<TResult>()
		{
			using (var readSession = ReadSession)
			{
				return readSession.QueryAs<T, TResult>(QueryBuilder.Build()).ToList();
			}
		}

		public override IList<string> ToListOfJson()
		{
			using (var readSession = ReadSession)
			{
				return readSession.QueryAsJson<T>(QueryBuilder.Build()).ToList();
			}
		}

		public override T Single()
		{
			using (var readSession = ReadSession)
			{
				return readSession.Query<T>(QueryBuilder.Build()).Single();
			}
		}

		public override TResult SingleAs<TResult>()
		{
			using (var readSession = ReadSession)
			{
				return readSession.QueryAs<T, TResult>(QueryBuilder.Build()).Single();
			}
		}

		public override string SingleAsJson()
		{
			using (var readSession = ReadSession)
			{
				return readSession.QueryAsJson<T>(QueryBuilder.Build()).Single();
			}
		}

		public override T SingleOrDefault()
		{
			using (var readSession = ReadSession)
			{
				return readSession.Query<T>(QueryBuilder.Build()).SingleOrDefault();
			}
		}

		public override TResult SingleOrDefaultAs<TResult>()
		{
			using (var readSession = ReadSession)
			{
				return readSession.QueryAs<T, TResult>(QueryBuilder.Build()).SingleOrDefault();
			}
		}

		public override string SingleOrDefaultAsJson()
		{
			using (var readSession = ReadSession)
			{
				return readSession.QueryAsJson<T>(QueryBuilder.Build()).SingleOrDefault();
			}
		}
	}
}