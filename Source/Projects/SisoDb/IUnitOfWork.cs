using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb
{
    /// <summary>
    /// All operations within the unit of work are transactional.
    /// Use <see cref="Commit"/> to make your changes persistant.
    /// </summary>
    public interface IUnitOfWork : IQueryEngine
    {
        /// <summary>
        /// Commits your changes to the database. After a commit you
        /// can continue to work with your UnitOfWork. You do not have
        /// to create a new instance of your UnitOfWork, as it will
        /// reset it self.
        /// </summary>
        void Commit();

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
        void InsertMany<T>(IList<T> items) where T : class;

        /// <summary>
        /// Inserts multiple Json strcutures using the <typeparamref name="T"/> as
        /// the contract for the structures being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structures will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        void InsertManyJson<T>(IList<string> json) where T : class;

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