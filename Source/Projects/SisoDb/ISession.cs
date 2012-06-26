using System;
using System.Collections.Generic;
using PineCone.Structures.Schemas;

namespace SisoDb
{
    /// <summary>
	/// A short lived sessioni used to interact with the database.
	/// </summary>
	public interface ISession : IDisposable
	{
        /// <summary>
        /// Unique identifier for the Session.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The database that this session is executed against.
        /// </summary>
	    ISisoDatabase Db { get; }

        /// <summary>
        /// Indicates the current status (<see cref="SessionStatus"/>) of the session.
        /// </summary>
	    SessionStatus Status { get; }

	    /// <summary>
		/// Low level API that executes queries as <see cref="IQuery"/>.
		/// </summary>
		IQueryEngine QueryEngine { get; }

		/// <summary>
		/// Advances querying options.
		/// </summary>
		IAdvanced Advanced { get; }

		/// <summary>
		/// Lets you get a hold of the schema associated with a certain structure.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IStructureSchema GetStructureSchema<T>() where T : class;

        /// <summary>
        /// Lets you get a hold of the schema associated with a certain structure.
        /// </summary>
        /// <param name="structureType"></param>
        /// <returns></returns>
        IStructureSchema GetStructureSchema(Type structureType);

        /// <summary>
        /// Lets you perform a Query defining things like
        /// <see cref="ISisoQueryable{T}.Take"/>
        /// <see cref="ISisoQueryable{T}.Where"/>
        /// <see cref="ISisoQueryable{T}.Include{TInclude}"/>
        /// <see cref="ISisoQueryable{T}.OrderBy"/>
        /// <see cref="ISisoQueryable{T}.Page"/>
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <returns></returns>
        /// <remarks>The query is defered and is executed when you start yield the result.</remarks>
        ISisoQueryable<T> Query<T>() where T : class;

        /// <summary>
        /// Returns (true) if there is a structure matching the sent <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
	    bool Exists<T>(object id) where T : class;

        /// <summary>
        /// Returns (true) if there is a structure matching the sent <paramref name="id"/>.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
	    bool Exists(Type structureType, object id);

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
        /// Returns one single structure identified
        /// by an id, as Json. This is the most
        /// effective return type, since the Json
        /// is stored in the database, no deserialization
        /// will take place.  
        /// </summary>
        /// <param name="structureType">
        /// Structure type, used as a contract defining the scheme.</param>
        /// <param name="id"></param>
        /// <returns>Json representation of (<param name="structureType"></param>) or Null</returns>
        string GetByIdAsJson(Type structureType, object id);

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
        /// Returns Json representation for all structures for the defined structure <param name="structureType"></param>
        /// matching passed ids.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable Json representation of <param name="structureType"></param>.</returns>
        IEnumerable<string> GetByIdsAsJson(Type structureType, params object[] ids);

		/// <summary>
        /// Inserts a single structure using the <typeparamref name="T"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="item"></param>
        ISession Insert<T>(T item) where T : class;

        /// <summary>
        /// Inserts a single structure using the <param name="structureType"></param> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <param name="structuretype"></param>
        /// <param name="item"></param>
        /// <returns></returns>
	    ISession Insert(Type structuretype, object item);

        /// <summary>
        /// Inserts a single structure using the <typeparamref name="T"/> as
        /// the contract for the structure being inserted. As item, you can pass
        /// any type that has partial or full match of the contract, without extending it.
        /// E.g An anonymous type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        ISession InsertAs<T>(object item) where T : class;

        /// <summary>
        /// Inserts a single structure using the <param name="structureType"></param> as
        /// the contract for the structure being inserted. As item, you can pass
        /// any type that has partial or full match of the contract, without extending it.
        /// E.g An anonymous type.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="item"></param>
        /// <returns></returns>
	    ISession InsertAs(Type structureType, object item);

        /// <summary>
        /// Inserts Json strcutures using the <typeparamref name="T"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structure will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns>The Json for the item being inserted, but after insert so that the Id is included.</returns>
        string InsertJson<T>(string json) where T : class;

	    /// <summary>
	    /// Inserts Json strcutures using the <param name="structureType"></param> as
	    /// the contract for the structure being inserted.
	    /// </summary>
	    /// <remarks>Deserialization of the Json structure will take place, 
	    /// so If you do have the instace pass that instead using other overload!</remarks>
	    /// <param name="structureType"></param>
	    /// <param name="json"></param>
	    /// <returns>The Json for the item being inserted, but after insert so that the Id is included.</returns>
	    string InsertJson(Type structureType, string json);

        /// <summary>
        /// Inserts multiple structures using the <typeparamref name="T"/> as
        /// the contract for the structures being inserted. 
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="items"></param>
        ISession InsertMany<T>(IEnumerable<T> items) where T : class;

        /// <summary>
        /// Inserts multiple structures using the <typeparamref name="T"/> as
        /// the contract for the structures being inserted. 
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="items"></param>
        /// <returns></returns>
	    ISession InsertMany(Type structureType, IEnumerable<object> items);

	    /// <summary>
	    /// Inserts multiple Json strcutures using the <typeparamref name="T"/> as
	    /// the contract for the structures being inserted.
	    /// </summary>
	    /// <remarks>Deserialization of the Json structures will take place, 
	    /// so If you do have the instace pass that instead using other overload!</remarks>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="json"></param>
	    /// <param name="onBatchInserted"></param>
	    void InsertManyJson<T>(IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null) where T : class;

        /// <summary>
        /// Inserts multiple Json strcutures using the <param name="structureType"></param> as
        /// the contract for the structures being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structures will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <param name="structureType"></param>
        /// <param name="json"></param>
        /// <param name="onBatchInserted"></param>
	    void InsertManyJson(Type structureType, IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null);

        /// <summary>
        /// Updates the sent structure. If it
        /// does not exist, an <see cref="SisoDbException"/> will be
        /// thrown.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="item"></param>
        ISession Update<T>(T item) where T : class;

        /// <summary>
        /// Updates the sent structure. If it
        /// does not exist, an <see cref="SisoDbException"/> will be
        /// thrown.
        /// </summary>
        /// <param name="structureType">
        /// Structure type, used as a contract defining the scheme.</param>
        /// <param name="item"></param>
        ISession Update(Type structureType, object item);

	    /// <summary>
	    /// Uses sent id to locate a structure and then calls sent <paramref name="modifier"/>
	    /// to apply the changes. Will also place an rowlock, which makes it highly
	    /// useful in a concurrent environment like in an event denormalizer.
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="id"></param>
	    /// <param name="modifier"></param>
        /// <param name="proceed">True to continue with update;False to abort</param>
        ISession Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class;

        /// <summary>
        /// Deletes structure by id.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="id"></param>
        ISession DeleteById<T>(object id) where T : class;

        /// <summary>
        /// Deletes structure by id.
        /// </summary>
        /// <param name="structureType">
        /// Structure type, used as a contract defining the scheme.</param>
        /// <param name="id"></param>
        ISession DeleteById(Type structureType, object id);

        /// <summary>
        /// Deletes all structures for the defined structure <typeparamref name="T"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids used for matching the structures to delete.</param>
        ISession DeleteByIds<T>(params object[] ids) where T : class;

	    /// <summary>
	    /// Deletes all structures for the defined structure <param name="structureType"></param>
	    /// matching passed ids.
	    /// </summary>
	    /// <param name="structureType">
	    /// Structure type, used as a contract defining the scheme.</param>
	    /// <param name="ids">Ids used for matching the structures to delete.</param>
        ISession DeleteByIds(Type structureType, params object[] ids);
	}
}