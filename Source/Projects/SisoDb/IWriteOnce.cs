using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb
{
	public interface IWriteOnce 
	{
		T Insert<T>(T item) where T : class;
		void InsertJson<T>(string json) where T : class;
        void InsertMany<T>(IEnumerable<T> items) where T : class;
        void InsertManyJson<T>(IEnumerable<string> json) where T : class;
		
		T Update<T>(T item) where T : class;
		void UpdateMany<T>(Expression<Func<T, bool>> expression, Action<T> modifier) where T : class;

		void DeleteById<T>(object id) where T : class;
		void DeleteByIds<T>(params object[] ids) where T : class;
		void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class;
		void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class;
	}
}