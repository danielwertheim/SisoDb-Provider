using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using SisoDb.Querying;

namespace SisoDb
{
    [DebuggerStepThrough]
    public class DbReadOnce : IReadOnce
    {
    	private readonly ISisoDbDatabase _db;

        public DbReadOnce(ISisoDbDatabase db)
        {
            _db = db;
        }

		public int Count<T>() where T : class
		{
			using (var session =_db.BeginReadSession())
			{
				return session.Query<T>().Count();
			}
		}

		public int Count<T>(Expression<Func<T, bool>> expression) where T : class
		{
			Ensure.That(expression, "expression").IsNotNull();

			return Query<T>().Count(expression);
		}

        public bool Exists<T>(object id) where T : class
        {
            Ensure.That(id, "id").IsNotNull();

            using (var session = _db.BeginReadSession())
            {
                return session.Exists<T>(id);
            } 
        }

        public T GetById<T>(object id) where T : class
		{
			Ensure.That(id, "id").IsNotNull();

			using (var session =_db.BeginReadSession())
			{
				return session.GetById<T>(id);
			}
		}

		public TOut GetByIdAs<TContract, TOut>(object id) where TContract : class where TOut : class
		{
			Ensure.That(id, "id").IsNotNull();

			using (var session =_db.BeginReadSession())
			{
				return session.GetByIdAs<TContract, TOut>(id);
			}
		}

    	public string GetByIdAsJson<T>(object id) where T : class
    	{
			Ensure.That(id, "id").IsNotNull();

    		using (var session =_db.BeginReadSession())
    		{
    			return session.GetByIdAsJson<T>(id);
    		}
    	}

    	public IList<T> GetByIds<T>(params object[] ids) where T : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var session =_db.BeginReadSession())
			{
				return session.GetByIds<T>(ids).ToList();
			}
		}

    	public IList<TOut> GetByIdsAs<TContract, TOut>(params object[] ids) where TContract : class where TOut : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var session =_db.BeginReadSession())
			{
				return session.GetByIdsAs<TContract, TOut>(ids).ToList();
			}
		}

    	public IList<string> GetByIdsAsJson<T>(params object[] ids) where T : class
    	{
    		Ensure.That(ids, "ids").HasItems();

    		using (var session =_db.BeginReadSession())
    		{
    			return session.GetByIdsAsJson<T>(ids).ToList();
    		}
    	}

    	public IList<T> GetByIdInterval<T>(object idFrom, object idTo) where T : class
		{
			Ensure.That(idFrom, "idFrom").IsNotNull();
			Ensure.That(idTo, "idTo").IsNotNull();

			using (var session =_db.BeginReadSession())
			{
				return session.GetByIdInterval<T>(idFrom, idTo).ToList();
			}
		}

    	public IList<T> NamedQuery<T>(INamedQuery query) where T : class
		{
			Ensure.That(query, "query").IsNotNull();

			using (var session =_db.BeginReadSession())
			{
				return session.Advanced.NamedQuery<T>(query).ToList();
			}
		}

		public IList<TOut> NamedQueryAs<TContract, TOut>(INamedQuery query) where TContract : class where TOut : class
		{
			Ensure.That(query, "query").IsNotNull();

			using (var session =_db.BeginReadSession())
			{
				return session.Advanced.NamedQueryAs<TContract, TOut>(query).ToList();
			}
		}

		public IList<string> NamedQueryAsJson<T>(INamedQuery query) where T : class
		{
			Ensure.That(query, "query").IsNotNull();

			using (var session =_db.BeginReadSession())
			{
				return session.Advanced.NamedQueryAsJson<T>(query).ToList();
			}
		}

		public IList<T> RawQuery<T>(IRawQuery query) where T : class
    	{
			Ensure.That(query, "query").IsNotNull();

			using (var session =_db.BeginReadSession())
			{
				return session.Advanced.RawQuery<T>(query).ToList();
			}
    	}

		public IList<TOut> RawQueryAs<TContract, TOut>(IRawQuery query) where TContract : class where TOut : class
    	{
			Ensure.That(query, "query").IsNotNull();

			using (var session =_db.BeginReadSession())
			{
				return session.Advanced.RawQueryAs<TContract, TOut>(query).ToList();
			}
    	}

		public IList<string> RawQueryAsJson<T>(IRawQuery query) where T : class
    	{
			Ensure.That(query, "query").IsNotNull();

			using (var session =_db.BeginReadSession())
			{
				return session.Advanced.RawQueryAsJson<T>(query).ToList();
			}
    	}

    	public ISisoQueryable<T> Query<T>() where T : class
		{
			return new SisoReadOnceQueryable<T>(_db.ProviderFactory.GetQueryBuilder<T>(_db.StructureSchemas), () => _db.BeginReadSession());
		}
    }
}