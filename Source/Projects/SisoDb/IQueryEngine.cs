using System;
using System.Collections.Generic;

namespace SisoDb
{
	public interface IQueryEngine : IDisposable
	{
		/// <summary>
		/// Advances querying options.
		/// </summary>
		IAdvancedQuerySession Advanced { get; }

		/// <summary>
		/// Issues a simple count for how many structures there are
		/// in the specified structure type <typeparamref name="T"/>,
		/// matching the where expression in <see cref="IQuery.Where"/>.
		/// </summary>
		/// <typeparam name="T">
		/// Structure type, used as a contract defining the scheme.</typeparam>
		/// <param name="query"></param>
		/// <returns>Number of structures.</returns>
		int Count<T>(IQuery query) where T : class;

		/// <summary>
		/// Returns one single structure identified
		/// by an id.
		/// </summary>
		/// <typeparam name="T">
		/// Structure type, used as a contract defining the scheme.</typeparam>
		/// <param name="id"></param>
		/// <returns>Structure (<typeparamref name="T"/>) or Null</returns>
		T GetById<T>(object id) where T : class;

		/// <summary>
		/// Returns one single structure identified
		/// by an id. 
		/// </summary>
		/// <typeparam name="TContract">
		/// Structure type, used as a contract defining the scheme.</typeparam>
		/// <typeparam name="TOut">
		/// Determines the type you want your structure deserialized to and returned as.</typeparam>
		/// <param name="id"></param>
		/// <returns>Structure (<typeparamref name="TOut"/>) or null.</returns>
		TOut GetByIdAs<TContract, TOut>(object id) where TContract : class where TOut : class;

		/// <summary>
		/// Returns all structures for the defined structure <typeparamref name="T"/>
		/// matching passed ids.
		/// </summary>
		/// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
		/// <param name="ids">Ids used for matching the structures to return.</param>
		/// <returns>IEnumerable of <typeparamref name="T"/>.</returns>
		IEnumerable<T> GetByIds<T>(params object[] ids) where T : class;

		/// <summary>
		/// Returns all structures for the defined structure <typeparamref name="TContract"/>
		/// matching passed ids.
		/// </summary>
		/// <typeparam name="TContract">
		/// Structure type, used as a contract defining the scheme.</typeparam>
		/// <typeparam name="TOut">
		/// Determines the type you want your structure deserialized to and returned as.</typeparam>
		/// <param name="ids">Ids used for matching the structures to return.</param>
		/// <returns>IEnumerable of <typeparamref name="TOut"/>.</returns>
		IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids) where TContract : class where TOut : class;

		/// <summary>
		/// Returns all structures for the defined structure <typeparamref name="T"/>
		/// matching specified id interval.
		/// </summary>
		/// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
		/// <param name="idFrom"></param>
		/// <param name="idTo"></param>
		/// <returns>IEnumerable of <typeparamref name="T"/>.</returns>
		/// /// <remarks>YOU COULD GET STRANGE RESULTS IF YOU HAVE SPECIFIED NON SEQUENTIAL GUIDS.</remarks>
		IEnumerable<T> GetByIdInterval<T>(object idFrom, object idTo) where T : class;

		/// <summary>
		/// Returns one single structure identified
		/// by an id, as Json. This is the most
		/// effective return type, since the Json
		/// is stored in the database, no deserialization
		/// will take place.  
		/// </summary>
		/// <typeparam name="T">
		/// Structure type, used as a contract defining the scheme.</typeparam>
		/// <param name="id"></param>
		/// <returns>Json representation of (<typeparamref name="T"/>) or Null</returns>
		string GetByIdAsJson<T>(object id) where T : class;

		/// <summary>
		/// Returns Json representation for all structures for the defined structure <typeparamref name="T"/>
		/// matching passed ids.
		/// </summary>
		/// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
		/// <param name="ids">Ids used for matching the structures to return.</param>
		/// <returns>IEnumerable Json representation of <typeparamref name="T"/>.</returns>
		IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class;

		/// <summary>
		/// Lets you perform a Query defining things like
		/// <see cref="ISisoQueryable{T}.Take"/>
		/// <see cref="ISisoQueryable{T}.Where"/>
		/// <see cref="ISisoQueryable{T}.Include{TInclude}"/>
		/// <see cref="ISisoQueryable{T}.OrderBy"/>
		/// </summary>
		/// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
		/// <returns></returns>
		/// <remarks>The query is defered and is executed when you start yield the result.</remarks>
		ISisoQueryable<T> Query<T>() where T : class;

		/// <summary>
		/// Lets you perform a Query by passing an <see cref="IQuery"/>.
		/// </summary>
		/// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
		/// <returns>IEnumerable of <typeparamref name="T"/>.</returns>
		/// <remarks>The query is defered and is executed when you start yield the result.</remarks>
		IEnumerable<T> Query<T>(IQuery query) where T : class;

		/// <summary>
		/// Lets you perform a Query by passing an <see cref="IQuery"/>.
		/// Returns structures for the defined structure <typeparamref name="T"/>
		/// deserialized as <typeparamref name="TResult"/>. 
		/// </summary>
		/// <typeparam name="T">
		/// Structure type, used as a contract defining the scheme.</typeparam>
		/// <typeparam name="TResult">
		/// Determines the type you want your structure deserialized to and returned as.</typeparam>
		/// <param name="query"></param>
		/// <returns>IEnumerable of <typeparamref name="TResult"/>.</returns>
		IEnumerable<TResult> QueryAs<T, TResult>(IQuery query)
			where T : class
			where TResult : class;

		/// <summary>
		/// Lets you perform a Query by passing an <see cref="IQuery"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <returns>IEnumerable Json representation of <typeparamref name="T"/>.</returns>
		IEnumerable<string> QueryAsJson<T>(IQuery query) where T : class;
	}
}