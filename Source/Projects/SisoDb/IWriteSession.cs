using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb
{
    /// <summary>
    /// Represents a Writable session. If the provider allows, it will be Transactional and
    /// implicitly committed when disposed.
    /// </summary>
    public interface IWriteSession : IReadSession
    {
        /// <summary>
        /// Inserts a single structure using the <typeparamref name="T"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="item"></param>
        void Insert<T>(T item) where T : class;

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