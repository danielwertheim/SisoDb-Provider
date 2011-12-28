using System;
using System.Collections.Generic;

namespace SisoDb
{
	/// <summary>
	/// Used to query the database.
	/// </summary>
	public interface IQueryEngine : IDisposable
	{
		/// <summary>
		/// Low level API that executes queries as <see cref="IQuery"/>.
		/// </summary>
		IQueryEngineCore Core { get; }

		/// <summary>
		/// Advances querying options.
		/// </summary>
		IAdvancedQueries Advanced { get; }

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
	}
}