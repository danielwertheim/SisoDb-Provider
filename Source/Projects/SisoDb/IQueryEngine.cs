using System.Collections.Generic;

namespace SisoDb
{
	public interface IQueryEngine
	{
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
	    /// Returns structures for the defined structure <typeparamref name="T"/>
	    /// deserialized as <typeparamref name="TResult"/> defined by the template. 
	    /// </summary>
	    /// <typeparam name="T">
	    /// Structure type, used as a contract defining the scheme.</typeparam>
	    /// <typeparam name="TResult">
	    /// Determines the type you want your structure deserialized to and returned as.</typeparam>
	    /// <param name="query"></param>
	    /// <param name="template"> </param>
	    /// <returns>IEnumerable of <typeparamref name="TResult"/>.</returns>
	    IEnumerable<TResult> QueryAsAnonymous<T, TResult>(IQuery query, TResult template)
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