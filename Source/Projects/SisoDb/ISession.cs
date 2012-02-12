using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PineCone.Structures.Schemas;

namespace SisoDb
{
	/// <summary>
	/// A short lived sessioni used to interact with the database.
	/// </summary>
	public interface ISession : IDisposable
	{
		/// <summary>
		/// Low level API that executes queries as <see cref="IQuery"/>.
		/// </summary>
		IQueryEngine QueryEngine { get; }

		/// <summary>
		/// Advances querying options.
		/// </summary>
		IAdvancedQueries Advanced { get; }

		/// <summary>
		/// Lets you get a hold of the schema associated with a certain structure.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IStructureSchema GetStructureSchema<T>() where T : class;

        /// <summary>
        /// Returns value indicating of structure exists or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exists<T>(object id) where T : class;

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
		/// Returns Json representation for all structures for the defined structure <typeparamref name="T"/>
		/// matching passed ids.
		/// </summary>
		/// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
		/// <param name="ids">Ids used for matching the structures to return.</param>
		/// <returns>IEnumerable Json representation of <typeparamref name="T"/>.</returns>
		IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class;

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
        /// Inserts a single structure using the <typeparamref name="T"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="item"></param>
        void Insert<T>(T item) where T : class;

        /// <summary>
        /// Inserts a single structure using the <typeparamref name="TContract"/> as
        /// the contract for the structure being inserted. As item, you can pass
        /// any type that has partial or full match of the contract, without extending it.
        /// E.g An anonymous type.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="item"></param>
	    void InsertTo<TContract>(object item) where TContract : class;

        /// <summary>
        /// Inserts Json strcutures using the <typeparamref name="T"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structure will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        void InsertJson<T>(string json) where T : class;

        /// <summary>
        /// Inserts multiple structures using the <typeparamref name="T"/> as
        /// the contract for the structures being inserted. 
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="items"></param>
        void InsertMany<T>(IEnumerable<T> items) where T : class;

        /// <summary>
        /// Inserts multiple Json strcutures using the <typeparamref name="T"/> as
        /// the contract for the structures being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structures will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        void InsertManyJson<T>(IEnumerable<string> json) where T : class;

        /// <summary>
        /// Updates the sent structure. If it
        /// does not exist, an <see cref="SisoDbException"/> will be
        /// thrown.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="item"></param>
        void Update<T>(T item) where T : class;

        /// <summary>
        /// Uses sent id to locate a structure and then calls sent <paramref name="modifier"/>
        /// to apply the changes. Useful to minimize the concurrency scope in
        /// e.g an event denormalizer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="modifier"></param>
        void Update<T>(object id, Action<T> modifier) where T : class;

        /// <summary>
        /// Traverses every structure in the set and lets you apply changes to each yielded structure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="modifier"></param>
        void UpdateMany<T>(Expression<Func<T, bool>> expression, Action<T> modifier) where T : class;

        /// <summary>
        /// Deletes structure by id.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="id"></param>
        void DeleteById<T>(object id) where T : class;

        /// <summary>
        /// Deletes all structures for the defined structure <typeparamref name="T"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids used for matching the structures to delete.</param>
        void DeleteByIds<T>(params object[] ids) where T : class;

        /// <summary>
        /// Deletes all structures for the defined structure <typeparamref name="T"/>
        /// having an id in the intervall.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="idFrom"></param>
        /// <param name="idTo"></param>
        /// <remarks>ONLY SUPPORTED BY IDENTITIES! If not sequenced ids are used, you could be doomed!</remarks>
        void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class;

        /// <summary>
        /// Deletes one or more structures matchings the sent
        /// expression.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="expression"></param>
        void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class;
	}
}