using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb
{
    /// <summary>
    /// Used to execute some more advances query operations against the database.
    /// </summary>
    public interface IAdvanced
    {
        /// <summary>
        /// Deletes one or more structures matchings the sent
        /// expression.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="expression"></param>
        /// <remarks>Does not notify the cache provider.</remarks>
        void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class;

        /// <summary>
        /// Traverses every structure in the set and lets you apply changes to each yielded structure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="modifier"></param>
        void UpdateMany<T>(Expression<Func<T, bool>> expression, Action<T> modifier) where T : class;

        /// <summary>
        /// Lets you invoke stored procedures that returns Json,
        /// which will get deserialized to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="query"></param>
        /// <returns>Empty or populated IEnumerable of <typeparamref name="T"/>.</returns>
        IEnumerable<T> NamedQuery<T>(INamedQuery query) where T : class;

        /// <summary>
        /// Lets you invoke stored procedures that returns Json,
        /// which will get deserialized to <typeparamref name="TOut"/>.
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="query"></param>
        /// <returns>Empty or populated IEnumerable of <typeparamref name="TOut"/>.</returns>
        IEnumerable<TOut> NamedQueryAs<TContract, TOut>(INamedQuery query) where TContract : class where TOut : class;

        /// <summary>
        /// Lets you invoke stored procedures that returns Json.
        /// This is the most effective return type, since the Json is stored in the database,
        /// no deserialization will take place. 
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="query"></param>
        /// <returns>Json representation of structures (<typeparamref name="T"/>)
        /// or empty IEnumerable of <see cref="string"/>.</returns>
        IEnumerable<string> NamedQueryAsJson<T>(INamedQuery query) where T : class;

		/// <summary>
		/// Lets you invoke raw query, e.g using SQL, that returns Json,
		/// which will get deserialized to <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">
		/// Structure type, used as a contract defining the scheme.</typeparam>
		/// <param name="query"></param>
		/// <returns>Empty or populated IEnumerable of <typeparamref name="T"/>.</returns>
		IEnumerable<T> RawQuery<T>(IRawQuery query) where T : class;

		/// <summary>
		/// Lets you invoke raw query, e.g using SQL, that returns Json,
		/// which will get deserialized to <typeparamref name="TOut"/>.
		/// </summary>
		/// <typeparam name="TContract">
		/// Structure type, used as a contract defining the scheme.</typeparam>
		/// <typeparam name="TOut">
		/// Determines the type you want your structure deserialized to and returned as.</typeparam>
		/// <param name="query"></param>
		/// <returns>Empty or populated IEnumerable of <typeparamref name="TOut"/>.</returns>
		IEnumerable<TOut> RawQueryAs<TContract, TOut>(IRawQuery query) where TContract : class where TOut : class;

		/// <summary>
		/// Lets you invoke a raw query, e.g using SQL, that returns Json.
		/// This is the most effective return type, since the Json is stored in the database,
		/// no deserialization will take place. 
		/// </summary>
		/// <typeparam name="T">
		/// Structure type, used as a contract defining the scheme.</typeparam>
		/// <param name="query"></param>
		/// <returns>Json representation of structures (<typeparamref name="T"/>)
		/// or empty IEnumerable of <see cref="string"/>.</returns>
		IEnumerable<string> RawQueryAsJson<T>(IRawQuery query) where T : class;
    }
}