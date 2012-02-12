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
    public class DbSingleOperationSession : ISingleOperationSession
    {
    	private readonly ISisoDbDatabase _db;

        public DbSingleOperationSession(ISisoDbDatabase db)
        {
            _db = db;
        }

		public int Count<T>() where T : class
		{
			using (var session =_db.BeginSession())
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

            using (var session = _db.BeginSession())
            {
                return session.Exists<T>(id);
            } 
        }

        public T GetById<T>(object id) where T : class
		{
			Ensure.That(id, "id").IsNotNull();

			using (var session =_db.BeginSession())
			{
				return session.GetById<T>(id);
			}
		}

		public TOut GetByIdAs<TContract, TOut>(object id) where TContract : class where TOut : class
		{
			Ensure.That(id, "id").IsNotNull();

			using (var session =_db.BeginSession())
			{
				return session.GetByIdAs<TContract, TOut>(id);
			}
		}

    	public string GetByIdAsJson<T>(object id) where T : class
    	{
			Ensure.That(id, "id").IsNotNull();

    		using (var session =_db.BeginSession())
    		{
    			return session.GetByIdAsJson<T>(id);
    		}
    	}

    	public T[] GetByIds<T>(params object[] ids) where T : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var session =_db.BeginSession())
			{
				return session.GetByIds<T>(ids).ToArray();
			}
		}

    	public TOut[] GetByIdsAs<TContract, TOut>(params object[] ids) where TContract : class where TOut : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var session =_db.BeginSession())
			{
				return session.GetByIdsAs<TContract, TOut>(ids).ToArray();
			}
		}

    	public string[] GetByIdsAsJson<T>(params object[] ids) where T : class
    	{
    		Ensure.That(ids, "ids").HasItems();

    		using (var session =_db.BeginSession())
    		{
    			return session.GetByIdsAsJson<T>(ids).ToArray();
    		}
    	}

    	public T[] GetByIdInterval<T>(object idFrom, object idTo) where T : class
		{
			Ensure.That(idFrom, "idFrom").IsNotNull();
			Ensure.That(idTo, "idTo").IsNotNull();

			using (var session =_db.BeginSession())
			{
				return session.GetByIdInterval<T>(idFrom, idTo).ToArray();
			}
		}

    	public T[] NamedQuery<T>(INamedQuery query) where T : class
		{
			Ensure.That(query, "query").IsNotNull();

			using (var session =_db.BeginSession())
			{
				return session.Advanced.NamedQuery<T>(query).ToArray();
			}
		}

		public TOut[] NamedQueryAs<TContract, TOut>(INamedQuery query) where TContract : class where TOut : class
		{
			Ensure.That(query, "query").IsNotNull();

			using (var session =_db.BeginSession())
			{
				return session.Advanced.NamedQueryAs<TContract, TOut>(query).ToArray();
			}
		}

		public string[] NamedQueryAsJson<T>(INamedQuery query) where T : class
		{
			Ensure.That(query, "query").IsNotNull();

			using (var session =_db.BeginSession())
			{
				return session.Advanced.NamedQueryAsJson<T>(query).ToArray();
			}
		}

		public T[] RawQuery<T>(IRawQuery query) where T : class
    	{
			Ensure.That(query, "query").IsNotNull();

			using (var session =_db.BeginSession())
			{
				return session.Advanced.RawQuery<T>(query).ToArray();
			}
    	}

		public TOut[] RawQueryAs<TContract, TOut>(IRawQuery query) where TContract : class where TOut : class
    	{
			Ensure.That(query, "query").IsNotNull();

			using (var session =_db.BeginSession())
			{
				return session.Advanced.RawQueryAs<TContract, TOut>(query).ToArray();
			}
    	}

		public string[] RawQueryAsJson<T>(IRawQuery query) where T : class
    	{
			Ensure.That(query, "query").IsNotNull();

			using (var session =_db.BeginSession())
			{
				return session.Advanced.RawQueryAsJson<T>(query).ToArray();
			}
    	}

    	public ISisoQueryable<T> Query<T>() where T : class
		{
			return new SisoReadOnceQueryable<T>(_db.ProviderFactory.GetQueryBuilder<T>(_db.StructureSchemas), () => _db.BeginSession());
		}

        public void Insert<T>(T item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = _db.BeginSession())
            {
                session.Insert(item);
            }
        }

        public void InsertTo<TContract>(object item) where TContract : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = _db.BeginSession())
            {
                session.InsertTo<TContract>(item);
            }
        }

        public void InsertJson<T>(string json) where T : class
        {
            Ensure.That(json, "json").IsNotNullOrWhiteSpace();

            using (var session = _db.BeginSession())
            {
                session.InsertJson<T>(json);
            }
        }

        public void InsertMany<T>(IEnumerable<T> items) where T : class
        {
            Ensure.That(items, "items").IsNotNull();

            using (var session = _db.BeginSession())
            {
                session.InsertMany(items);
            }
        }

        public void InsertManyJson<T>(IEnumerable<string> json) where T : class
        {
            Ensure.That(json, "json").IsNotNull();

            using (var session = _db.BeginSession())
            {
                session.InsertManyJson<T>(json);
            }
        }

        public T Update<T>(T item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = _db.BeginSession())
            {
                session.Update(item);
            }

            return item;
        }

        public void UpdateMany<T>(Expression<Func<T, bool>> expression, Action<T> modifier) where T : class
        {
            Ensure.That(expression, "expression").IsNotNull();
            Ensure.That(modifier, "modifier").IsNotNull();

            using (var session = _db.BeginSession())
                session.UpdateMany(expression, modifier);
        }

        public void DeleteById<T>(object id) where T : class
        {
            Ensure.That(id, "id").IsNotNull();

            using (var session = _db.BeginSession())
            {
                session.DeleteById<T>(id);
            }
        }

        public void DeleteByIds<T>(params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = _db.BeginSession())
            {
                session.DeleteByIds<T>(ids);
            }
        }

        public void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class
        {
            Ensure.That(idFrom, "idFrom").IsNotNull();
            Ensure.That(idTo, "idTo").IsNotNull();

            using (var session = _db.BeginSession())
            {
                session.DeleteByIdInterval<T>(idFrom, idTo);
            }
        }

        public void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
        {
            Ensure.That(expression, "expression").IsNotNull();

            using (var session = _db.BeginSession())
            {
                session.DeleteByQuery(expression);
            }
        }
    }
}