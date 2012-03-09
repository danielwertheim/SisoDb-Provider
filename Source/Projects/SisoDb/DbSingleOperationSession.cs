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

        public void InsertAs<T>(object item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = _db.BeginSession())
            {
                session.InsertAs<T>(item);
            }
        }

        public string InsertJson<T>(string json) where T : class
        {
            Ensure.That(json, "json").IsNotNullOrWhiteSpace();

            using (var session = _db.BeginSession())
            {
                return session.InsertJson<T>(json);
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

        public void InsertManyJson<T>(IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null) where T : class
        {
            Ensure.That(json, "json").IsNotNull();

            using (var session = _db.BeginSession())
            {
                session.InsertManyJson<T>(json, onBatchInserted);
            }
        }

        public void Update<T>(T item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = _db.BeginSession())
            {
                session.Update(item);
            }
        }

        public void Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class
        {
            Ensure.That(id, "id").IsNotNull();
            Ensure.That(modifier, "modifier").IsNotNull();

            using (var session = _db.BeginSession())
            {
                session.Update(id, modifier, proceed);
            }
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
    }
}