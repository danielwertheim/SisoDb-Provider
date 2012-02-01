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

			using (var session = _db.BeginWriteSession())
			{
				session.Insert(item);
			}

			return item;
		}

		public void InsertJson<T>(string json) where T : class
		{
			Ensure.That(json, "json").IsNotNullOrWhiteSpace();

			using (var session = _db.BeginWriteSession())
			{
				session.InsertJson<T>(json);
			}
		}

        public void InsertMany<T>(IEnumerable<T> items) where T : class
		{
            Ensure.That(items, "items").IsNotNull();

			using (var session = _db.BeginWriteSession())
			{
				session.InsertMany(items);
			}
		}

        public void InsertManyJson<T>(IEnumerable<string> json) where T : class
		{
			Ensure.That(json, "json").IsNotNull();

			using (var session = _db.BeginWriteSession())
			{
				session.InsertManyJson<T>(json);
			}
		}

		public T Update<T>(T item) where T : class
		{
			Ensure.That(item, "item").IsNotNull();

			using (var session = _db.BeginWriteSession())
			{
				session.Update(item);
			}

			return item;
		}

		public void UpdateMany<T>(Expression<Func<T, bool>> expression, Action<T> modifier) where T : class
		{
			Ensure.That(expression, "expression").IsNotNull();
			Ensure.That(modifier, "modifier").IsNotNull();

			using (var session = _db.BeginWriteSession())
				session.UpdateMany(expression, modifier);
		}

		public void DeleteById<T>(object id) where T : class
		{
			Ensure.That(id, "id").IsNotNull();

			using (var session = _db.BeginWriteSession())
			{
				session.DeleteById<T>(id);
			}
		}

		public void DeleteByIds<T>(params object[] ids) where T : class
		{
			Ensure.That(ids, "ids").HasItems();

			using (var session = _db.BeginWriteSession())
			{
				session.DeleteByIds<T>(ids);
			}
		}

		public void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class
		{
			Ensure.That(idFrom, "idFrom").IsNotNull();
			Ensure.That(idTo, "idTo").IsNotNull();

			using (var session = _db.BeginWriteSession())
				session.DeleteByIdInterval<T>(idFrom, idTo);
		}

		public void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
		{
			Ensure.That(expression, "expression").IsNotNull();

			using (var session = _db.BeginWriteSession())
			{
				session.DeleteByQuery(expression);
			}
		}
	}
}