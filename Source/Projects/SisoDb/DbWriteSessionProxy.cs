using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Resources;

namespace SisoDb
{
	[DebuggerStepThrough]
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

			if (Exception != null)
			{
				InnerSession.DisposeWhenFailed();
				InnerSession = null;
				throw Exception;
			}

			InnerSession.Dispose();
			InnerSession = null;
		}

		[DebuggerStepThrough]
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

		[DebuggerStepThrough]
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

		[DebuggerStepThrough]
		int IQueryEngine.Count<T>(IQuery query)
		{
			return ExecuteOperation(() => InnerSession.QueryEngine.Count<T>(query));
		}

		[DebuggerStepThrough]
		IEnumerable<T> IQueryEngine.Query<T>(IQuery query)
		{
			return ExecuteOperation(() => InnerSession.QueryEngine.Query<T>(query));
		}

		[DebuggerStepThrough]
		IEnumerable<TResult> IQueryEngine.QueryAs<T, TResult>(IQuery query)
		{
			return ExecuteOperation(() => InnerSession.QueryEngine.QueryAs<T, TResult>(query));
		}

		[DebuggerStepThrough]
		IEnumerable<string> IQueryEngine.QueryAsJson<T>(IQuery query)
		{
			return ExecuteOperation(() => InnerSession.QueryEngine.QueryAsJson<T>(query));
		}

		[DebuggerStepThrough]
		IEnumerable<T> IAdvancedQueries.NamedQuery<T>(INamedQuery query)
		{
			return ExecuteOperation(() => InnerSession.Advanced.NamedQuery<T>(query));
		}

		[DebuggerStepThrough]
		IEnumerable<TOut> IAdvancedQueries.NamedQueryAs<TContract, TOut>(INamedQuery query)
		{
			return ExecuteOperation(() => InnerSession.Advanced.NamedQueryAs<TContract, TOut>(query));
		}

		[DebuggerStepThrough]
		IEnumerable<string> IAdvancedQueries.NamedQueryAsJson<T>(INamedQuery query)
		{
			return ExecuteOperation(() => InnerSession.Advanced.NamedQueryAsJson<T>(query));
		}

		[DebuggerStepThrough]
		IEnumerable<T> IAdvancedQueries.RawQuery<T>(IRawQuery query)
		{
			return ExecuteOperation(() => InnerSession.Advanced.RawQuery<T>(query));
		}

		[DebuggerStepThrough]
		IEnumerable<TOut> IAdvancedQueries.RawQueryAs<TContract, TOut>(IRawQuery query)
		{
			return ExecuteOperation(() => InnerSession.Advanced.RawQueryAs<TContract, TOut>(query));
		}

		[DebuggerStepThrough]
		IEnumerable<string> IAdvancedQueries.RawQueryAsJson<T>(IRawQuery query)
		{
			return ExecuteOperation(() => InnerSession.Advanced.RawQueryAsJson<T>(query));
		}

		[DebuggerStepThrough]
		public IStructureSchema GetStructureSchema<T>() where T : class
		{
			return ExecuteOperation(() => InnerSession.GetStructureSchema<T>());
		}

		[DebuggerStepThrough]
		public T GetById<T>(object id) where T : class
		{
			return ExecuteOperation(() => InnerSession.GetById<T>(id));
		}

		[DebuggerStepThrough]
		public TOut GetByIdAs<TContract, TOut>(object id)
			where TContract : class
			where TOut : class
		{
			return ExecuteOperation(() => InnerSession.GetByIdAs<TContract, TOut>(id));
		}

		[DebuggerStepThrough]
		public string GetByIdAsJson<T>(object id) where T : class
		{
			return ExecuteOperation(() => InnerSession.GetByIdAsJson<T>(id));
		}

		[DebuggerStepThrough]
		public IEnumerable<T> GetByIds<T>(params object[] ids) where T : class
		{
			return ExecuteOperation(() => InnerSession.GetByIds<T>(ids));
		}

		[DebuggerStepThrough]
		public IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids)
			where TContract : class
			where TOut : class
		{
			return ExecuteOperation(() => InnerSession.GetByIdsAs<TContract, TOut>(ids));
		}

		[DebuggerStepThrough]
		public IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class
		{
			return ExecuteOperation(() => InnerSession.GetByIdsAsJson<T>(ids));
		}

		[DebuggerStepThrough]
		public IEnumerable<T> GetByIdInterval<T>(object idFrom, object idTo) where T : class
		{
			return ExecuteOperation(() => InnerSession.GetByIdInterval<T>(idFrom, idTo));
		}

		[DebuggerStepThrough]
		public ISisoQueryable<T> Query<T>() where T : class
		{
			return ExecuteOperation(() => InnerSession.Query<T>());
		}

		[DebuggerStepThrough]
		public void Insert<T>(T item) where T : class
		{
			ExecuteOperation(() => InnerSession.Insert(item));
		}

		[DebuggerStepThrough]
		public void InsertJson<T>(string json) where T : class
		{
			ExecuteOperation(() => InnerSession.InsertJson<T>(json));
		}

		[DebuggerStepThrough]
        public void InsertMany<T>(IEnumerable<T> items) where T : class
		{
			ExecuteOperation(() => InnerSession.InsertMany(items));
		}

		[DebuggerStepThrough]
		public void InsertManyJson<T>(IEnumerable<string> json) where T : class
		{
			ExecuteOperation(() => InnerSession.InsertManyJson<T>(json));
		}

		[DebuggerStepThrough]
		public void Update<T>(T item) where T : class
		{
			ExecuteOperation(() => InnerSession.Update(item));
		}

		[DebuggerStepThrough]
		public void UpdateMany<T>(Expression<Func<T, bool>> expression, Action<T> modifier) where T : class
		{
			ExecuteOperation(() => InnerSession.UpdateMany(expression, modifier));
		}

		[DebuggerStepThrough]
		public void DeleteById<T>(object id) where T : class
		{
			ExecuteOperation(() => InnerSession.DeleteById<T>(id));
		}

		[DebuggerStepThrough]
		public void DeleteByIds<T>(params object[] ids) where T : class
		{
			ExecuteOperation(() => InnerSession.DeleteByIds<T>(ids));
		}

		[DebuggerStepThrough]
		public void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class
		{
			ExecuteOperation(() => InnerSession.DeleteByIdInterval<T>(idFrom, idTo));
		}

		[DebuggerStepThrough]
		public void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
		{
			ExecuteOperation(() => InnerSession.DeleteByQuery(expression));
		}
	}
}