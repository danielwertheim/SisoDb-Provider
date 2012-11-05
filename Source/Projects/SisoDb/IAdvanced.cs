using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Dac;
using SisoDb.Querying;

namespace SisoDb
{
    /// <summary>
    /// Used to execute some more advances query operations against the database.
    /// </summary>
    public interface IAdvanced
    {
        /// <summary>
        /// Lets you run Non Query SQL against the Db.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        void NonQuery(string sql, params IDacParameter[] parameters);

        /// <summary>
        /// Lets you upsert a named query (stored procedure).
        /// If one allready exists, it will be dropped first.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="spec"></param>
        void UpsertNamedQuery<T>(string name, Action<IQueryBuilder<T>> spec) where T : class;

        /// <summary>
        /// Deletes one or more structures matchings the sent
        /// predicate.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="predicate"></param>
        void DeleteByQuery<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Lets you invoke a stored procedures that returns Json,
        /// which will get deserialized to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="query"></param>
        /// <returns>Empty or populated IEnumerable of <typeparamref name="T"/>.</returns>
        /// <remarks>Does not consume the cache provider.</remarks>
        IEnumerable<T> NamedQuery<T>(INamedQuery query) where T : class;

        /// <summary>
        /// Lets you invoke a stored procedures that returns Json,
        /// which will get deserialized to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="name">Name of the Stored procedure</param>
        /// <param name="predicate">Used to populate the arguments of the underlying <see cref="INamedQuery"/>.</param>
        /// <returns>Empty or populated IEnumerable of <typeparamref name="T"/>.</returns>
        /// <remarks>Does not consume the cache provider.</remarks>
        IEnumerable<T> NamedQuery<T>(string name, Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Lets you invoke a stored procedures that returns Json,
        /// which will get deserialized to <typeparamref name="TOut"/>.
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="query"></param>
        /// <returns>Empty or populated IEnumerable of <typeparamref name="TOut"/>.</returns>
        /// <remarks>Does not consume the cache provider.</remarks>
        IEnumerable<TOut> NamedQueryAs<TContract, TOut>(INamedQuery query)
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Lets you invoke a stored procedures that returns Json,
        /// which will get deserialized to <typeparamref name="TOut"/>.
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="name">Name of the Stored procedure</param>
        /// <param name="predicate">Used to populate the arguments of the underlying <see cref="INamedQuery"/>.</param>
        /// <returns>Empty or populated IEnumerable of <typeparamref name="TOut"/>.</returns>
        /// <remarks>Does not consume the cache provider.</remarks>
        IEnumerable<TOut> NamedQueryAs<TContract, TOut>(string name, Expression<Func<TContract, bool>> predicate)
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Lets you invoke a stored procedures that returns Json.
        /// This is the most effective return type, since the Json is stored in the database,
        /// no deserialization will take place. 
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="query"></param>
        /// <returns>Json representation of structures (<typeparamref name="T"/>)
        /// or empty IEnumerable of <see cref="string"/>.</returns>
        /// <remarks>Does not consume the cache provider.</remarks>
        IEnumerable<string> NamedQueryAsJson<T>(INamedQuery query) where T : class;

        /// <summary>
        /// Lets you invoke a stored procedures that returns Json.
        /// This is the most effective return type, since the Json is stored in the database,
        /// no deserialization will take place. 
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="name">Name of the Stored procedure</param>
        /// <param name="predicate">Used to populate the arguments of the underlying <see cref="INamedQuery"/>.</param>
        /// <returns>Json representation of structures (<typeparamref name="T"/>)
        /// or empty IEnumerable of <see cref="string"/>.</returns>
        /// <remarks>Does not consume the cache provider.</remarks>
        IEnumerable<string> NamedQueryAsJson<T>(string name, Expression<Func<T, bool>> predicate) where T : class;

		/// <summary>
		/// Lets you invoke a raw query, e.g using SQL, that returns Json,
		/// which will get deserialized to <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">
		/// Structure type, used as a contract defining the scheme.</typeparam>
		/// <param name="query"></param>
		/// <returns>Empty or populated IEnumerable of <typeparamref name="T"/>.</returns>
        /// <remarks>Does not consume the cache provider.</remarks>
		IEnumerable<T> RawQuery<T>(IRawQuery query) where T : class;

		/// <summary>
		/// Lets you invoke a raw query, e.g using SQL, that returns Json,
		/// which will get deserialized to <typeparamref name="TOut"/>.
		/// </summary>
		/// <typeparam name="TContract">
		/// Structure type, used as a contract defining the scheme.</typeparam>
		/// <typeparam name="TOut">
		/// Determines the type you want your structure deserialized to and returned as.</typeparam>
		/// <param name="query"></param>
		/// <returns>Empty or populated IEnumerable of <typeparamref name="TOut"/>.</returns>
        /// <remarks>Does not consume the cache provider.</remarks>
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
        /// <remarks>Does not consume the cache provider.</remarks>
		IEnumerable<string> RawQueryAsJson<T>(IRawQuery query) where T : class;

        /// <summary>
        /// Traverses every structure in the set and lets you apply changes to each yielded structure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="modifier"></param>
        /// <remarks>Does not support Concurrency tokens</remarks>
        void UpdateMany<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class; //TODO: Move
    }
}