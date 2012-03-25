using System;
using System.Collections.Generic;

namespace SisoDb
{
	public interface ISingleOperationSession 
	{
        ISisoQueryable<T> Query<T>() where T : class;

	    T GetById<T>(object id) where T : class;
	    TOut GetByIdAs<TContract, TOut>(object id) where TContract : class where TOut : class;
	    
        string GetByIdAsJson<T>(object id) where T : class;
        string GetByIdAsJson(Type structureType, object id);

	    T[] GetByIds<T>(params object[] ids) where T : class;
	    TOut[] GetByIdsAs<TContract, TOut>(params object[] ids) where TContract : class where TOut : class;
	    
        string[] GetByIdsAsJson<T>(params object[] ids) where T : class;
        IEnumerable<string> GetByIdsAsJson(Type structureType, params object[] ids);

	    void Insert<T>(T item) where T : class;
        void InsertAs<T>(object item) where T : class;
	    
        string InsertJson<T>(string json) where T : class;
        string InsertJson(Type structureType, string json);

	    void InsertMany<T>(IEnumerable<T> items) where T : class;
        void InsertManyJson<T>(IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null) where T : class;
        void InsertManyJson(Type structureType, IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null);

	    void Update<T>(T item) where T : class;
        void Update(Type structureType, object item);
        void Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class;

	    void DeleteById<T>(object id) where T : class;
        void DeleteById(Type structureType, object id);

	    void DeleteByIds<T>(params object[] ids) where T : class;
        void DeleteByIds(Type structureType, params object[] ids);
	}
}