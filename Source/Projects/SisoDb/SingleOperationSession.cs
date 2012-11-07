using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.EnsureThat;
using SisoDb.Querying;

namespace SisoDb
{
    [DebuggerStepThrough]
    public class SingleOperationSession : ISingleOperationSession
    {
        protected readonly ISisoDatabase Db;

        public SingleOperationSession(ISisoDatabase db)
        {
            Ensure.That(db, "db").IsNotNull();
            Db = db;
        }

        public virtual ISisoQueryable<T> Query<T>() where T : class
        {
            return new SisoReadOnceQueryable<T>(
                Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas), 
                () => Db.BeginSession());
        }

        public virtual T GetById<T>(object id) where T : class
		{
			Ensure.That(id, "id").IsNotNull();

			using (var session = Db.BeginSession())
			{
				return session.GetById<T>(id);
			}
		}

        public virtual object GetById(Type structureType, object id)
        {
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return session.GetById(structureType, id);
            }
        }

        public virtual TOut GetByIdAs<TContract, TOut>(object id)
            where TContract : class
            where TOut : class
		{
			Ensure.That(id, "id").IsNotNull();

			using (var session = Db.BeginSession())
			{
				return session.GetByIdAs<TContract, TOut>(id);
			}
		}

        public virtual string GetByIdAsJson<T>(object id) where T : class
    	{
			Ensure.That(id, "id").IsNotNull();

    		using (var session = Db.BeginSession())
    		{
    			return session.GetByIdAsJson<T>(id);
    		}
    	}

        public virtual string GetByIdAsJson(Type structureType, object id)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                return session.GetByIdAsJson(structureType, id);
            }
        }

        public virtual T[] GetByIds<T>(params object[] ids) where T : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var session = Db.BeginSession())
			{
				return session.GetByIds<T>(ids).ToArray();
			}
		}

        public virtual object[] GetByIds(Type structureType, params object[] ids)
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                return session.GetByIds(structureType, ids).ToArray();
            }
        }

        public virtual TOut[] GetByIdsAs<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var session = Db.BeginSession())
			{
				return session.GetByIdsAs<TContract, TOut>(ids).ToArray();
			}
		}

        public virtual string[] GetByIdsAsJson<T>(params object[] ids) where T : class
    	{
    		Ensure.That(ids, "ids").HasItems();

    		using (var session = Db.BeginSession())
    		{
    			return session.GetByIdsAsJson<T>(ids).ToArray();
    		}
    	}

        public virtual IEnumerable<string> GetByIdsAsJson(Type structureType, params object[] ids)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                return session.GetByIdsAsJson(structureType, ids);
            }
        }

        public virtual void Insert<T>(T item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.Insert(item);
            }
        }

        public virtual void Insert(Type structureType, object item)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.Insert(structureType, item);
            }
        }

        public virtual void InsertAs<T>(object item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.InsertAs<T>(item);
            }
        }

        public virtual void InsertAs(Type structureType, object item)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(item, "item").IsNotNull();

            using(var session = Db.BeginSession())
            {
                session.InsertAs(structureType, item);
            }
        }

        public virtual string InsertJson<T>(string json) where T : class
        {
            Ensure.That(json, "json").IsNotNullOrWhiteSpace();

            using (var session = Db.BeginSession())
            {
                return session.InsertJson<T>(json);
            }
        }

        public virtual string InsertJson(Type structureType, string json)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(json, "json").IsNotNullOrWhiteSpace();

            using (var session = Db.BeginSession())
            {
                return session.InsertJson(structureType, json);
            }
        }

        public virtual void InsertMany<T>(IEnumerable<T> items) where T : class
        {
            Ensure.That(items, "items").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.InsertMany(items);
            }
        }

        public virtual void InsertMany(Type structureType, IEnumerable<object> items)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(items, "items").IsNotNull();

            using(var session = Db.BeginSession())
            {
                session.InsertMany(structureType, items);
            }
        }

        public virtual void InsertManyJson<T>(IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null) where T : class
        {
            Ensure.That(json, "json").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.InsertManyJson<T>(json, onBatchInserted);
            }
        }

        public virtual void InsertManyJson(Type structureType, IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(json, "json").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.InsertManyJson(structureType, json, onBatchInserted);
            }
        }

        public virtual void Update<T>(T item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.Update(item);
            }
        }

        public virtual void Update(Type structureType, object item)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(item, "item").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.Update(structureType, item);
            }
        }

        public virtual void Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class
        {
            Ensure.That(id, "id").IsNotNull();
            Ensure.That(modifier, "modifier").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.Update(id, modifier, proceed);
            }
        }

        public virtual void UpdateMany<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();
            Ensure.That(modifier, "modifier").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.UpdateMany(predicate, modifier);
            }
        }

        public virtual void Clear<T>() where T : class
        {
            using (var session = Db.BeginSession())
            {
                session.Clear<T>();
            }
        }

        public virtual void Clear(Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.Clear(structureType);
            }
        }

        public virtual void DeleteAllExceptIds<T>(params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                session.DeleteAllExceptIds<T>(ids);
            }
        }

        public virtual void DeleteAllExceptIds(Type structureType, params object[] ids)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                session.DeleteAllExceptIds(structureType, ids);
            }
        }

        public virtual void DeleteById<T>(object id) where T : class
        {
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.DeleteById<T>(id);
            }
        }

        public virtual void DeleteById(Type structureType, object id)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(id, "id").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.DeleteById(structureType, id);
            }
        }

        public virtual void DeleteByIds<T>(params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                session.DeleteByIds<T>(ids);
            }
        }

        public virtual void DeleteByIds(Type structureType, params object[] ids)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(ids, "ids").HasItems();

            using (var session = Db.BeginSession())
            {
                session.DeleteByIds(structureType, ids);
            }
        }

        public virtual void DeleteByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();

            using (var session = Db.BeginSession())
            {
                session.DeleteByQuery(predicate);
            }
        }
    }
}