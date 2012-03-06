using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb
{
	public interface ISingleOperationSession 
	{
	    int Count<T>() where T : class;
	    int Count<T>(Expression<Func<T, bool>> expression) where T : class;

	    bool Exists<T>(object id) where T : class;

	    T GetById<T>(object id) where T : class;
	    TOut GetByIdAs<TContract, TOut>(object id) where TContract : class where TOut : class;
	    string GetByIdAsJson<T>(object id) where T : class;
	    T[] GetByIds<T>(params object[] ids) where T : class;
	    TOut[] GetByIdsAs<TContract, TOut>(params object[] ids) where TContract : class where TOut : class;
	    string[] GetByIdsAsJson<T>(params object[] ids) where T : class;

	    ISisoQueryable<T> Query<T>() where T : class;

	    void Insert<T>(T item) where T : class;
        void InsertAs<T>(object item) where T : class;
	    string InsertJson<T>(string json) where T : class;
	    void InsertMany<T>(IEnumerable<T> items) where T : class;
        void InsertManyJson<T>(IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null) where T : class;

	    void Update<T>(T item) where T : class;
        void Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class;

	    void DeleteById<T>(object id) where T : class;
	    void DeleteByIds<T>(params object[] ids) where T : class;
	}
}