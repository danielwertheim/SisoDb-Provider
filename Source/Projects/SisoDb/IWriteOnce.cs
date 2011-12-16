using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb
{
	public interface IWriteOnce {
		T Insert<T>(T item) where T : class;
		void InsertJson<T>(string json) where T : class;
		IList<T> InsertMany<T>(IList<T> items) where T : class;
		void InsertManyJson<T>(IList<string> json) where T : class;
		T Update<T>(T item) where T : class;
		bool UpdateMany<T>(Func<T, UpdateManyModifierStatus> modifier, Expression<Func<T, bool>> expression = null) where T : class;

		bool UpdateMany<TOld, TNew>(Func<TOld, TNew, UpdateManyModifierStatus> modifier, Expression<Func<TOld, bool>> expression = null)
			where TOld : class
			where TNew : class;

		void DeleteById<T>(object id) where T : class;
		void DeleteByIds<T>(params object[] ids) where T : class;
		void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class;
		void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class;
	}
}