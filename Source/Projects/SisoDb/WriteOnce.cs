using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using EnsureThat;

namespace SisoDb
{
    [DebuggerStepThrough]
    public class WriteOnce : IWriteOnce
    {
    	private readonly ISisoDatabase _db;

        public WriteOnce(ISisoDatabase db)
        {
            _db = db;
        }

		public T Insert<T>(T item) where T : class
		{
			Ensure.That(item, "item").IsNotNull();

			using (var uow = _db.CreateUnitOfWork())
			{
				uow.Insert(item);
				uow.Commit();
			}

			return item;
		}

		public void InsertJson<T>(string json) where T : class
		{
			Ensure.That(json, "json").IsNotNullOrWhiteSpace();

			using (var uow = _db.CreateUnitOfWork())
			{
				uow.InsertJson<T>(json);
				uow.Commit();
			}
		}

		public IList<T> InsertMany<T>(IList<T> items) where T : class
		{
			Ensure.That(items, "items").HasItems();

			using (var uow = _db.CreateUnitOfWork())
			{
				uow.InsertMany(items);
				uow.Commit();
			}

			return items;
		}

		public void InsertManyJson<T>(IList<string> json) where T : class
		{
			Ensure.That(json, "json").HasItems();

			using (var uow = _db.CreateUnitOfWork())
			{
				uow.InsertManyJson<T>(json);
				uow.Commit();
			}
		}

		public T Update<T>(T item) where T : class
		{
			Ensure.That(item, "item").IsNotNull();

			using (var uow = _db.CreateUnitOfWork())
			{
				uow.Update(item);
				uow.Commit();
			}

			return item;
		}

		public bool UpdateMany<T>(Func<T, UpdateManyModifierStatus> modifier, Expression<Func<T, bool>> expression = null) where T : class
		{
			Ensure.That(modifier, "modifier").IsNotNull();
			bool result;

			using (var uow = _db.CreateUnitOfWork())
			{
				result = uow.UpdateMany(modifier, expression);
				
				if(result)
					uow.Commit();
			}

			return result;
		}

		public bool UpdateMany<TOld, TNew>(Func<TOld, TNew, UpdateManyModifierStatus> modifier, Expression<Func<TOld, bool>> expression = null)
			where TOld : class
			where TNew : class
		{
			Ensure.That(modifier, "modifier").IsNotNull();
			bool result;

			using (var uow = _db.CreateUnitOfWork())
			{
				result = uow.UpdateMany(modifier, expression);

				if (result)
					uow.Commit();
			}

			return result;
		}

		public void DeleteById<T>(object id) where T : class
		{
			using (var uow = _db.CreateUnitOfWork())
			{
				uow.DeleteById<T>(id);
				uow.Commit();
			}
		}

		public void DeleteByIds<T>(params object[] ids) where T : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var uow = _db.CreateUnitOfWork())
			{
				uow.DeleteByIds<T>(ids);
				uow.Commit();
			}
		}

		public void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class
		{
			using (var uow = _db.CreateUnitOfWork())
			{
				uow.DeleteByIdInterval<T>(idFrom, idTo);
				uow.Commit();
			}
		}

		public void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
		{
			using (var uow = _db.CreateUnitOfWork())
			{
				uow.DeleteByQuery(expression);
				uow.Commit();
			}
		}
    }
}