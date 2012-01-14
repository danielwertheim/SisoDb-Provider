using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using EnsureThat;

namespace SisoDb
{
    [DebuggerStepThrough]
    public class DbWriteOnce : IWriteOnce
    {
    	private readonly ISisoDbDatabase _db;

        public DbWriteOnce(ISisoDbDatabase db)
        {
            _db = db;
        }

		public T Insert<T>(T item) where T : class
		{
			Ensure.That(item, "item").IsNotNull();

			using (var uow = _db.BeginWriteSession())
			{
				uow.Insert(item);
			}

			return item;
		}

		public void InsertJson<T>(string json) where T : class
		{
			Ensure.That(json, "json").IsNotNullOrWhiteSpace();

			using (var uow = _db.BeginWriteSession())
			{
				uow.InsertJson<T>(json);
			}
		}

		public IList<T> InsertMany<T>(IList<T> items) where T : class
		{
			Ensure.That(items, "items").HasItems();

			using (var uow = _db.BeginWriteSession())
			{
				uow.InsertMany(items);
			}

			return items;
		}

		public void InsertManyJson<T>(IList<string> json) where T : class
		{
			Ensure.That(json, "json").HasItems();

			using (var uow = _db.BeginWriteSession())
			{
				uow.InsertManyJson<T>(json);
			}
		}

		public T Update<T>(T item) where T : class
		{
			Ensure.That(item, "item").IsNotNull();

			using (var uow = _db.BeginWriteSession())
			{
				uow.Update(item);
			}

			return item;
		}

		public void UpdateMany<T>(Func<T, UpdateManyModifierStatus> modifier, Expression<Func<T, bool>> expression = null) where T : class
		{
			Ensure.That(modifier, "modifier").IsNotNull();

			using (var uow = _db.BeginWriteSession())
			{
				uow.UpdateMany(modifier, expression);
			}
		}

		public void UpdateMany<TOld, TNew>(Func<TOld, TNew, UpdateManyModifierStatus> modifier, Expression<Func<TOld, bool>> expression = null)
			where TOld : class
			where TNew : class
		{
			Ensure.That(modifier, "modifier").IsNotNull();

			using (var uow = _db.BeginWriteSession())
			{
				uow.UpdateMany(modifier, expression);
			}
		}

		public void DeleteById<T>(object id) where T : class
		{
			using (var uow = _db.BeginWriteSession())
			{
				uow.DeleteById<T>(id);
			}
		}

		public void DeleteByIds<T>(params object[] ids) where T : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var uow = _db.BeginWriteSession())
			{
				uow.DeleteByIds<T>(ids);
			}
		}

		public void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class
		{
			using (var uow = _db.BeginWriteSession())
			{
				uow.DeleteByIdInterval<T>(idFrom, idTo);
			}
		}

		public void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
		{
			using (var uow = _db.BeginWriteSession())
			{
				uow.DeleteByQuery(expression);
			}
		}
    }
}