using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Querying;

namespace SisoDb
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();

        void Insert<T>(T item) where T : class;

        void InsertMany<T>(IEnumerable<T> items) where T : class;

        void Update<T>(T item) where T : class;

        void DeleteById<T>(Guid id) where T : class;

        void DeleteById<T>(int id) where T : class;

        void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class;

        int Count<T>() where T : class;

        T GetById<T>(Guid id) where T : class;

        T GetById<T>(int id) where T : class;

        TOut GetByIdAs<T, TOut>(Guid id)
            where T : class
            where TOut : class;

        TOut GetByIdAs<T, TOut>(int id)
            where T : class
            where TOut : class;

        string GetByIdAsJson<T>(Guid id) where T : class;

        string GetByIdAsJson<T>(int id) where T : class;

        IEnumerable<T> GetAll<T>() where T : class;

        IEnumerable<T> GetAll<T>(Action<IGetCommand<T>> commandInitializer) where T : class;

        IEnumerable<TOut> GetAllAs<T, TOut>()
            where T : class
            where TOut : class;

        IEnumerable<TOut> GetAllAs<T, TOut>(Action<IGetCommand<T>> commandInitializer)
            where T : class
            where TOut : class;

        IEnumerable<string> GetAllAsJson<T>() where T : class;

        IEnumerable<string> GetAllAsJson<T>(Action<IGetCommand<T>> commandInitializer) where T : class;

        IEnumerable<T> NamedQuery<T>(INamedQuery query) where T : class;

        IEnumerable<TOut> NamedQueryAs<T, TOut>(INamedQuery query)
            where T : class
            where TOut : class;

        IEnumerable<string> NamedQueryAsJson<T>(INamedQuery query) where T : class;

        IEnumerable<T> Where<T>(Expression<Func<T, bool>> expression) where T : class;

        IEnumerable<TOut> WhereAs<T, TOut>(Expression<Func<T, bool>> expression)
            where T : class
            where TOut : class;

        IEnumerable<string> WhereAsJson<T>(Expression<Func<T, bool>> expression) where T : class;

        IEnumerable<T> Query<T>(Action<IQueryCommand<T>> commandInitializer) where T : class;
        
        IEnumerable<TOut> QueryAs<T, TOut>(Action<IQueryCommand<T>> commandInitializer)
            where T : class
            where TOut : class;

        IEnumerable<string> QueryAsJson<T>(Action<IQueryCommand<T>> commandInitializer) where T : class;
    }
}