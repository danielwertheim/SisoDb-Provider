using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.EnsureThat;
using SisoDb.Resources;

namespace SisoDb.Querying
{
    public class SisoReadOnceQueryable<T> : ISisoQueryable<T> where T : class 
	{
        protected readonly Func<ISession> SessionFactory;
        protected readonly IQueryBuilder<T> QueryBuilder;

		public SisoReadOnceQueryable(IQueryBuilder<T> queryBuilder, Func<ISession> sessionFactory)
		{
		    Ensure.That(queryBuilder, "queryBuilder").IsNotNull();
            Ensure.That(sessionFactory, "sessionFactory").IsNotNull();

		    QueryBuilder = queryBuilder;
            SessionFactory = sessionFactory;
		}

        public virtual bool Any()
        {
            using (var session = SessionFactory.Invoke())
            {
                return QueryBuilder.IsEmpty
                    ? session.QueryEngine.Any<T>()
                    : session.QueryEngine.Any<T>(QueryBuilder.Build());
            }
        }

        public virtual bool Any(Expression<Func<T, bool>> expression)
        {
            Ensure.That(expression, "expression").IsNotNull();

            using (var session = SessionFactory.Invoke())
            {
                QueryBuilder.Clear();
                QueryBuilder.Where(expression);

                return session.QueryEngine.Any<T>(QueryBuilder.Build());
            }
        }

		public virtual int Count()
		{
			using (var session = SessionFactory.Invoke())
			{
                return QueryBuilder.IsEmpty
                    ? session.QueryEngine.Count<T>()
                    : session.QueryEngine.Count<T>(QueryBuilder.Build());
			}
		}

		public virtual int Count(Expression<Func<T, bool>> expression)
		{
		    Ensure.That(expression, "expression").IsNotNull();

			using (var session = SessionFactory.Invoke())
			{
                QueryBuilder.Clear();
			    QueryBuilder.Where(expression);

                return session.QueryEngine.Count<T>(QueryBuilder.Build());
			}
		}

        public virtual bool Exists(object id)
        {
            Ensure.That(id, "id").IsNotNull();

            using(var session = SessionFactory.Invoke())
            {
                return session.QueryEngine.Exists<T>(id);
            }
        }
		
		public virtual T First()
		{
			using (var session = SessionFactory.Invoke())
			{
				return session.QueryEngine.Query<T>(QueryBuilder.Build()).First();
			}
		}

        public virtual TResult FirstAs<TResult>() where TResult : class
		{
			using (var session = SessionFactory.Invoke())
			{
				return session.QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build()).First();
			}
		}

		public virtual string FirstAsJson()
		{
			using (var session = SessionFactory.Invoke())
			{
				return session.QueryEngine.QueryAsJson<T>(QueryBuilder.Build()).First();
			}
		}

		public virtual T FirstOrDefault()
		{
			using (var session = SessionFactory.Invoke())
			{
				return session.QueryEngine.Query<T>(QueryBuilder.Build()).FirstOrDefault();
			}
		}

        public virtual TResult FirstOrDefaultAs<TResult>() where TResult : class
		{
			using (var session = SessionFactory.Invoke())
			{
				return session.QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build()).FirstOrDefault();
			}
		}

		public virtual string FirstOrDefaultAsJson()
		{
			using (var session = SessionFactory.Invoke())
			{
				return session.QueryEngine.QueryAsJson<T>(QueryBuilder.Build()).FirstOrDefault();
			}
		}

        public virtual T Single()
        {
            using (var session = SessionFactory.Invoke())
            {
                return session.QueryEngine.Query<T>(QueryBuilder.Build()).Single();
            }
        }

        public virtual TResult SingleAs<TResult>() where TResult : class
        {
            using (var session = SessionFactory.Invoke())
            {
                return session.QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build()).Single();
            }
        }

        public virtual string SingleAsJson()
        {
            using (var session = SessionFactory.Invoke())
            {
                return session.QueryEngine.QueryAsJson<T>(QueryBuilder.Build()).Single();
            }
        }

        public virtual T SingleOrDefault()
        {
            using (var session = SessionFactory.Invoke())
            {
                return session.QueryEngine.Query<T>(QueryBuilder.Build()).SingleOrDefault();
            }
        }

        public virtual TResult SingleOrDefaultAs<TResult>() where TResult : class
        {
            using (var session = SessionFactory.Invoke())
            {
                return session.QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build()).SingleOrDefault();
            }
        }

        public virtual string SingleOrDefaultAsJson()
        {
            using (var session = SessionFactory.Invoke())
            {
                return session.QueryEngine.QueryAsJson<T>(QueryBuilder.Build()).SingleOrDefault();
            }
        }

		public virtual T[] ToArray()
		{
			using (var session = SessionFactory.Invoke())
			{
				return session.QueryEngine.Query<T>(QueryBuilder.Build()).ToArray();
			}
		}

        public virtual TResult[] ToArrayOf<TResult>() where TResult : class
		{
			using (var session = SessionFactory.Invoke())
			{
				return session.QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build()).ToArray();
			}
		}

        public virtual TResult[] ToArrayOf<TResult>(TResult template) where TResult : class
        {
            using (var session = SessionFactory.Invoke())
            {
                return session.QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build()).ToArray();
            }
        }

        public virtual TResult[] ToArrayOf<TResult>(Expression<Func<T, TResult>> projection) where TResult : class
        {
            using (var session = SessionFactory.Invoke())
            {
                return session.QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build()).ToArray();
            }
        }

        public virtual string[] ToArrayOfJson()
		{
			using (var session = SessionFactory.Invoke())
			{
				return session.QueryEngine.QueryAsJson<T>(QueryBuilder.Build()).ToArray();
			}
		}

		public virtual IEnumerable<T> ToEnumerable()
		{
			throw new SisoDbException(ExceptionMessages.ReadOnceQueryable_YieldingNotSupported);
		}

        public virtual IEnumerable<TResult> ToEnumerableOf<TResult>() where TResult : class
		{
			throw new SisoDbException(ExceptionMessages.ReadOnceQueryable_YieldingNotSupported);
		}

        public virtual IEnumerable<TResult> ToEnumerableOf<TResult>(TResult template) where TResult : class
	    {
	        throw new SisoDbException(ExceptionMessages.ReadOnceQueryable_YieldingNotSupported);
	    }

        public virtual IEnumerable<TResult> ToEnumerableOf<TResult>(Expression<Func<T, TResult>> projection) where TResult : class
        {
            throw new SisoDbException(ExceptionMessages.ReadOnceQueryable_YieldingNotSupported);
        }

        public virtual IEnumerable<string> ToEnumerableOfJson()
		{
			throw new SisoDbException(ExceptionMessages.ReadOnceQueryable_YieldingNotSupported);
		}

	    public virtual IList<T> ToList()
		{
			using (var session = SessionFactory.Invoke())
			{
				return session.QueryEngine.Query<T>(QueryBuilder.Build()).ToList();
			}
		}

        public virtual IList<TResult> ToListOf<TResult>() where TResult : class
		{
			using (var session = SessionFactory.Invoke())
			{
				return session.QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build()).ToList();
			}
		}

        public virtual IList<TResult> ToListOf<TResult>(TResult template) where TResult : class
        {
            using (var session = SessionFactory.Invoke())
            {
                return session.QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build()).ToList();
            }
        }

        public virtual IList<TResult> ToListOf<TResult>(Expression<Func<T, TResult>> projection) where TResult : class
        {
            using (var session = SessionFactory.Invoke())
            {
                return session.QueryEngine.QueryAs<T, TResult>(QueryBuilder.Build()).ToList();
            }
        }

        public virtual IList<string> ToListOfJson()
		{
			using (var session = SessionFactory.Invoke())
			{
				return session.QueryEngine.QueryAsJson<T>(QueryBuilder.Build()).ToList();
			}
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

        //TODO: Rem for v16.0.0 final
        //public virtual ISisoQueryable<T> Include<TInclude>(params Expression<Func<T, object>>[] expressions) where TInclude : class
        //{
        //    QueryBuilder.Include<TInclude>(expressions);

        //    return this;
        //}

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