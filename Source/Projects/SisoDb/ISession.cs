using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Dac;
using SisoDb.Structures.Schemas;

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
        /// If you want to extend the <see cref="ISession"/> you should
        /// wrap your code withing <see cref="ISessionExecutionContext.Try"/>
        /// so that exceptions will set the session in failed state.
        /// </summary>
        ISessionExecutionContext ExecutionContext { get; }

        /// <summary>
        /// The database that this session is executed against.
        /// </summary>
        ISisoDatabase Db { get; }

        /// <summary>
        /// Manages all raw querying, inserting etc. against the DB. Be careful.
        /// </summary>
        IDbClient DbClient { get; }

        /// <summary>
        /// Indicates the current status (<see cref="SessionStatus"/>) of the session.
        /// </summary>
        SessionStatus Status { get; }

        /// <summary>
        /// Indicates that the session has been aborted as a result of
        /// an explicit call to <see cref="Abort"/>.
        /// </summary>
        bool IsAborted { get; }

        /// <summary>
        /// Indicates that the session has failed as a result of
        /// an exception or call to <see cref="MarkAsFailed"/>.
        /// </summary>
        bool HasFailed { get; }

        /// <summary>
        /// Provides some hoos like <see cref="ISessionEvents.OnInserted"/>.
        /// </summary>
        ISessionEvents Events { get; }

        /// <summary>
        /// Low level API that executes queries as <see cref="IQuery"/>.
        /// </summary>
        IQueryEngine QueryEngine { get; }

        /// <summary>
        /// Advances querying options.
        /// </summary>
        IAdvanced Advanced { get; }

        /// <summary>
        /// Marks the session as aborted. When aborted, and transactions are supported,
        /// commit will not be performed. Do not use it to fail a session. Then use
        /// <see cref="MarkAsFailed"/> instead.
        /// </summary>
        void Abort();

        /// <summary>
        /// Marks the session as failed. When failed, and transactions are supported,
        /// commit will not be performed. Do not use it to abort a session. Then use
        /// <see cref="Abort"/> instead.
        /// </summary>
        void MarkAsFailed();

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
        /// <see cref="ISisoQueryable{T}.OrderBy"/>
        /// <see cref="ISisoQueryable{T}.Page"/>
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <returns></returns>
        /// <remarks>The query is defered and is executed when you start yield the result.</remarks>
        ISisoQueryable<T> Query<T>() where T : class;

        /// <summary>
        /// Returns bool indicating if the specified structure <typeparamref name="T"/>,
        /// has any items at all.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <returns>Number of structures.</returns>
        bool Any<T>() where T : class;

        /// <summary>
        /// Returns bool indicating if the specified structure <paramref name="structureType" />,
        /// has any items at all.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <returns>Number of structures.</returns>
        bool Any(Type structureType);

        /// <summary>
        /// Issues a simple count for how many structures there
        /// are in the specified structure <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <returns></returns>
        int Count<T>() where T : class;

        /// <summary>
        /// Issues a simple count for how many structures there
        /// are in the specified structure <paramref name="structureType"/>.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <returns></returns>
        int Count(Type structureType);

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
        /// Returns the structure or NULL representing the sent <paramref name="id"/>.
        /// The item returned is fetched with exclusive row locks, hence keep the
        /// scope of the session as small as possible.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        T CheckoutById<T>(object id) where T : class;

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
        /// Returns one single structure identified by an id.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="id"></param>
        /// <returns>Structure for (<paramref name="structureType"/>) matching <paramref name="id"/> or NULL.</returns>
        object GetById(Type structureType, object id);

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
        TOut GetByIdAs<TContract, TOut>(object id)
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Returns one single structure identified
        /// by an id. 
        /// </summary>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="structureType"></param>
        /// <param name="id"></param>
        /// <returns>Structure for (<paramref name="structureType"/>) as (<typeparamref name="TOut"/>) or null.</returns>
        TOut GetByIdAs<TOut>(Type structureType, object id)
            where TOut : class;

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
        /// <returns>Json representation of (<paramref name="structureType"/>) or Null</returns>
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
        /// Returns all structures for the defined structure type <paramref name="structureType"/>
        /// that matches passed ids.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        object[] GetByIds(Type structureType, params object[] ids);

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="TContract"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>Structures for (<typeparamref name="TContract"/>) as (Enumerable of <typeparamref name="TOut"/>).</returns>
        IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Returns all structures for the defined structure <paramref name="structureType"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="structureType"></param>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>Structures for (<paramref name="structureType"/>) as (Enumerable of <typeparamref name="TOut"/>).</returns>
        IEnumerable<TOut> GetByIdsAs<TOut>(Type structureType, params object[] ids)
            where TOut : class;

        /// <summary>
        /// Returns Json representation for all structures for the defined structure <typeparamref name="T"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable Json representation of <typeparamref name="T"/>.</returns>
        IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class;

        /// <summary>
        /// Returns Json representation for all structures for the defined structure <paramref name="structureType"/>
        /// matching passed ids.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable Json representation of <paramref name="structureType"/>.</returns>
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
        /// Inserts a single structure using the <paramref name="structureType" /> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        ISession Insert(Type structureType, object item);

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
        /// Inserts a single structure using the <paramref name="structureType"/> as
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
        /// Inserts Json strcutures using the <paramref name="structureType"/> as
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
        /// Inserts multiple structures using the <paramref name="structureType" /> as
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
        void InsertManyJson<T>(IEnumerable<string> json) where T : class;

        /// <summary>
        /// Inserts multiple Json strcutures using the <paramref name="structureType" /> as
        /// the contract for the structures being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structures will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <param name="structureType"></param>
        /// <param name="json"></param>
        void InsertManyJson(Type structureType, IEnumerable<string> json);

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
        /// <returns></returns>
        ISession Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class;

        /// <summary>
        /// Uses sent id to locate a structure and then calls sent <paramref name="modifier"/>
        /// to apply the changes. Will also place an rowlock, which makes it highly
        /// useful in a concurrent environment like in an event denormalizer.
        /// </summary>
        /// <typeparam name="TContract">Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <param name="id"></param>
        /// <param name="modifier"></param>
        /// <param name="proceed">True to continue with update;False to abort</param>
        /// <returns></returns>
        ISession Update<TContract, TImpl>(object id, Action<TImpl> modifier, Func<TImpl, bool> proceed = null)
            where TContract : class
            where TImpl : class;

        /// <summary>
        /// Traverses every structure in the set and lets you apply changes to each yielded structure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="modifier"></param>
        /// <remarks>Does not support Concurrency tokens</remarks>
        /// <returns></returns>
        ISession UpdateMany<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class;

        /// <summary>
        /// Clears all stored structures of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ISession Clear<T>() where T : class;

        /// <summary>
        /// Clears all stored structures of type specified by <paramref name="structureType"/>.
        /// </summary>
        /// <param name="structureType"></param>
        /// <returns></returns>
        ISession Clear(Type structureType);

        /// <summary>
        /// Deletes all items except items having an id present in <paramref name="ids"/>.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids for the structures to keep.</param>
        /// <returns></returns>
        ISession DeleteAllExceptIds<T>(params object[] ids) where T : class;

        /// <summary>
        /// Deletes all items except items having an id present in <paramref name="ids"/>.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <param name="ids"></param>
        /// <returns></returns>
        ISession DeleteAllExceptIds(Type structureType, params object[] ids);

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
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
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
        /// Deletes all structures for the defined structure <paramref name="structureType"/>
        /// matching passed ids.
        /// </summary>
        /// <param name="structureType">
        /// Structure type, used as a contract defining the scheme.</param>
        /// <param name="ids">Ids used for matching the structures to delete.</param>
        ISession DeleteByIds(Type structureType, params object[] ids);

        /// <summary>
        /// Deletes one or more structures matchings the sent
        /// predicate.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="predicate"></param>
        ISession DeleteByQuery<T>(Expression<Func<T, bool>> predicate) where T : class;
    }
}