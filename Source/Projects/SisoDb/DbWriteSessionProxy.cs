using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Resources;

namespace SisoDb
{
	public class DbWriteSessionProxy : IWriteSession, IQueryEngine, IAdvancedQueries
	{
		protected DbWriteSession InnerSession;
		protected Exception Exception;

		public DbWriteSessionProxy(DbWriteSession session)
		{
			Ensure.That(session, "session").IsNotNull();

			InnerSession = session;
		}

		public virtual void Dispose()
		{
			GC.SuppressFinalize(this);

			if (InnerSession == null)
				throw new ObjectDisposedException(ExceptionMessages.WriteSession_AllreadyDisposed);

			if (Exception == null)
			{
				InnerSession.Dispose();
				InnerSession = null;
				return;
			}

			InnerSession.DisposeWhenFailed();
			InnerSession = null;

			throw Exception;
		}

		protected virtual void ExecuteOperation(Action operation)
		{
			try
			{
				operation();
			}
			catch (Exception ex)
			{
				Exception = ex;
				
				throw;
			}
		}

		protected virtual T ExecuteOperation<T>(Func<T> operation)
		{
			try
			{
				return operation();
			}
			catch (Exception ex)
			{
				Exception = ex;

				throw;
			}
		}

		public IQueryEngine QueryEngine
		{
			get { return this; }
		}

		public IAdvancedQueries Advanced
		{
			get { return this; }
		}

		int IQueryEngine.Count<T>(IQuery query)
		{
			return ExecuteOperation(() => InnerSession.QueryEngine.Count<T>(query));
		}

		IEnumerable<T> IQueryEngine.Query<T>(IQuery query)
		{
			return ExecuteOperation(() => InnerSession.QueryEngine.Query<T>(query));
		}

		IEnumerable<TResult> IQueryEngine.QueryAs<T, TResult>(IQuery query)
		{
			return ExecuteOperation(() => InnerSession.QueryEngine.QueryAs<T, TResult>(query));
		}

		IEnumerable<string> IQueryEngine.QueryAsJson<T>(IQuery query)
		{
			return ExecuteOperation(() => InnerSession.QueryEngine.QueryAsJson<T>(query));
		}

		IEnumerable<T> IAdvancedQueries.NamedQuery<T>(INamedQuery query)
		{
			return ExecuteOperation(() => InnerSession.Advanced.NamedQuery<T>(query));
		}

		IEnumerable<TOut> IAdvancedQueries.NamedQueryAs<TContract, TOut>(INamedQuery query)
		{
			return ExecuteOperation(() => InnerSession.Advanced.NamedQueryAs<TContract, TOut>(query));
		}

		IEnumerable<string> IAdvancedQueries.NamedQueryAsJson<T>(INamedQuery query)
		{
			return ExecuteOperation(() => InnerSession.Advanced.NamedQueryAsJson<T>(query));
		}

		IEnumerable<T> IAdvancedQueries.RawQuery<T>(IRawQuery query)
		{
			return ExecuteOperation(() => InnerSession.Advanced.RawQuery<T>(query));
		}

		IEnumerable<TOut> IAdvancedQueries.RawQueryAs<TContract, TOut>(IRawQuery query)
		{
			return ExecuteOperation(() => InnerSession.Advanced.RawQueryAs<TContract, TOut>(query));
		}

		IEnumerable<string> IAdvancedQueries.RawQueryAsJson<T>(IRawQuery query)
		{
			return ExecuteOperation(() => InnerSession.Advanced.RawQueryAsJson<T>(query));
		}

		public IStructureSchema GetStructureSchema<T>() where T : class
		{
			return ExecuteOperation(() => InnerSession.GetStructureSchema<T>());
		}

		public T GetById<T>(object id) where T : class
		{
			return ExecuteOperation(() => InnerSession.GetById<T>(id));
		}

		public TOut GetByIdAs<TContract, TOut>(object id)
			where TContract : class
			where TOut : class
		{
			return ExecuteOperation(() => InnerSession.GetByIdAs<TContract, TOut>(id));
		}

		public string GetByIdAsJson<T>(object id) where T : class
		{
			return ExecuteOperation(() => InnerSession.GetByIdAsJson<T>(id));
		}

		public IEnumerable<T> GetByIds<T>(params object[] ids) where T : class
		{
			return ExecuteOperation(() => InnerSession.GetByIds<T>(ids));
		}

		public IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids)
			where TContract : class
			where TOut : class
		{
			return ExecuteOperation(() => InnerSession.GetByIdsAs<TContract, TOut>(ids));
		}

		public IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class
		{
			return ExecuteOperation(() => InnerSession.GetByIdsAsJson<T>(ids));
		}

		public IEnumerable<T> GetByIdInterval<T>(object idFrom, object idTo) where T : class
		{
			return ExecuteOperation(() => InnerSession.GetByIdInterval<T>(idFrom, idTo));
		}

		public ISisoQueryable<T> Query<T>() where T : class
		{
			return ExecuteOperation(() => InnerSession.Query<T>());
		}

		public void Insert<T>(T item) where T : class
		{
			ExecuteOperation(() => InnerSession.Insert(item));
		}

		public void InsertJson<T>(string json) where T : class
		{
			ExecuteOperation(() => InnerSession.InsertJson<T>(json));
		}

		public void InsertMany<T>(IList<T> items) where T : class
		{
			ExecuteOperation(() => InnerSession.InsertMany(items));
		}

		public void InsertManyJson<T>(IEnumerable<string> json) where T : class
		{
			ExecuteOperation(() => InnerSession.InsertManyJson<T>(json));
		}

		public void Update<T>(T item) where T : class
		{
			ExecuteOperation(() => InnerSession.Update(item));
		}

		public void UpdateMany<T>(Func<T, UpdateManyModifierStatus> modifier, Expression<Func<T, bool>> expression = null) where T : class
		{
			ExecuteOperation(() => InnerSession.UpdateMany(modifier, expression));
		}

		public void UpdateMany<TOld, TNew>(Func<TOld, TNew, UpdateManyModifierStatus> modifier, Expression<Func<TOld, bool>> expression = null)
			where TOld : class
			where TNew : class
		{
			ExecuteOperation(() => InnerSession.UpdateMany(modifier, expression));
		}

		public void DeleteById<T>(object id) where T : class
		{
			ExecuteOperation(() => InnerSession.DeleteById<T>(id));
		}

		public void DeleteByIds<T>(params object[] ids) where T : class
		{
			ExecuteOperation(() => InnerSession.DeleteByIds<T>(ids));
		}

		public void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class
		{
			ExecuteOperation(() => InnerSession.DeleteByIdInterval<T>(idFrom, idTo));
		}

		public void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
		{
			ExecuteOperation(() => InnerSession.DeleteByQuery(expression));
		}
	}
}