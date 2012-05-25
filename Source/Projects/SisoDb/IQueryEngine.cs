using System;
using System.Collections.Generic;

namespace SisoDb
{
	public interface IQueryEngine
	{
        /// <summary>
        /// Returns bool indicating if the specified structure <typeparamref name="T"/>,
        /// has any items at all.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <returns>Number of structures.</returns>
        bool Any<T>() where T : class;

        /// <summary>
        /// Returns bool indicating if the specified structure <param name="structureType"></param>,
        /// has any items at all.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <returns>Number of structures.</returns>
        bool Any(Type structureType);

        /// <summary>
        /// Returns bool indicating if the specified structure <typeparamref name="T"/>,
        /// has any items matching the where expression in <see cref="IQuery.Where"/>.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="query"></param>
        /// <returns>Number of structures.</returns>
        bool Any<T>(IQuery query) where T : class;

        /// <summary>
        /// Returns bool indicating if the specified structure <param name="structureType"></param>,
        /// has any items matching the where expression in <see cref="IQuery.Where"/>.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <param name="query"></param>
        /// <returns>Number of structures.</returns>
        bool Any(Type structureType, IQuery query);

        /// <summary>
        /// Issues a simple count for how many structures there
        /// are in the specified structure <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <returns></returns>
        int Count<T>() where T : class;

        /// <summary>
        /// Issues a simple count for how many structures there
        /// are in the specified structure <param name="structureType"></param>.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <returns></returns>
        int Count(Type structureType);

        /// <summary>
        /// Issues a simple count for how many structures there are
        /// in the specified structure <typeparamref name="T"/>,
        /// matching the where expression in <see cref="IQuery.Where"/>.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="query"></param>
        /// <returns>Number of structures.</returns>
        int Count<T>(IQuery query) where T : class;

	    /// <summary>
	    /// Issues a simple count for how many structures there are
        /// in the specified structure <param name="structureType"></param>,
	    /// matching the where expression in <see cref="IQuery.Where"/>.
	    /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
	    /// <param name="query"></param>
	    /// <returns>Number of structures.</returns>
	    int Count(Type structureType, IQuery query);

        /// <summary>
        /// Returns value indicating of structure exists or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists<T>(object id) where T : class;

        /// <summary>
        /// Returns value indicating of structure exists or not.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists(Type structureType, object id);

        /// <summary>
		/// Lets you perform a Query by passing an <see cref="IQuery"/>.
		/// </summary>
		/// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="query"></param>
		/// <returns>IEnumerable of <typeparamref name="T"/>.</returns>
		/// <remarks>The query is defered and is executed when you start yield the result.</remarks>
		IEnumerable<T> Query<T>(IQuery query) where T : class;

        /// <summary>
        /// Lets you perform a Query by passing an <see cref="IQuery"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <returns>IEnumerable of object, representing <param name="structureType"></param>.</returns>
        /// <remarks>The query is defered and is executed when you start yield the result.</remarks>
        IEnumerable<object> Query(IQuery query, Type structureType);

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
		IEnumerable<TResult> QueryAs<T, TResult>(IQuery query) where T : class where TResult : class;

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
	    /// <param name="template"></param>
	    /// <returns>IEnumerable of <typeparamref name="TResult"/>.</returns>
	    IEnumerable<TResult> QueryAsAnonymous<T, TResult>(IQuery query, TResult template) where T : class where TResult : class;
        
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
        /// <param name="templateType"></param>
        /// <returns>IEnumerable of <typeparamref name="TResult"/>.</returns>
        IEnumerable<TResult> QueryAsAnonymous<T, TResult>(IQuery query, Type templateType) where T : class where TResult : class;

		/// <summary>
		/// Lets you perform a Query by passing an <see cref="IQuery"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <returns>IEnumerable Json representation of <typeparamref name="T"/>.</returns>
		IEnumerable<string> QueryAsJson<T>(IQuery query) where T : class;

	    /// <summary>
	    /// Lets you perform a Query by passing an <see cref="IQuery"/>.
	    /// </summary>
        /// <param name="query"></param>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <returns>IEnumerable Json representation of <param name="structureType"></param>.</returns>
        IEnumerable<string> QueryAsJson(IQuery query, Type structureType);
	}
}