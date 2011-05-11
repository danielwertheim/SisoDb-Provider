using System;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    /// <summary>
    /// Represents a database. An instance of
    /// a database is designed for being long lived, since
    /// it contains cache for structure schemas etc.
    /// </summary>
    public interface ISisoDatabase
    {
        /// <summary>
        /// The name of the database.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Connection info for the Server.
        /// </summary>
        ISisoConnectionInfo ServerConnectionInfo { get; }

        /// <summary>
        /// Connection info for the database.
        /// </summary>
        ISisoConnectionInfo ConnectionInfo { get; }

        /// <summary>
        /// Cached structure schemas.
        /// </summary>
        IStructureSchemas StructureSchemas { get; set; }

        /// <summary>
        /// Structure builder used to build structures for insert and updates.
        /// </summary>
        IStructureBuilder StructureBuilder { get; set; }

        /// <summary>
        /// Ensures that a new fresh database will exists. Drops any existing database.
        /// </summary>
        void EnsureNewDatabase();

        /// <summary>
        /// Creates and initializes a new database if one does not exist.
        /// </summary>
        void CreateIfNotExists();

        /// <summary>
        /// Initializes an existing database by creating Siso-system tables. 
        /// </summary>
        void InitializeExisting();

        /// <summary>
        /// Deletes the databse if it exists.
        /// </summary>
        void DeleteIfExists();

        /// <summary>
        /// Checks if the database exists.
        /// </summary>
        /// <returns></returns>
        bool Exists();

        /// <summary>
        /// Drops the structure set, meaning all tables associated with
        /// the structure type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void DropStructureSet<T>() where T : class;

        /// <summary>
        /// Manually upserts a structure set, meaning all tables for
        /// the structure type will be created.
        /// </summary>
        /// <remarks>
        /// This is normally something you do not need to do.
        /// This is done automatically.</remarks>
        /// <typeparam name="T"></typeparam>
        void UpsertStructureSet<T>() where T : class;

        /// <summary>
        /// Lets you handle complex model updates.
        /// </summary>
        /// <seealso cref="http://sisodb.com/docs/doc13"/>
        /// <typeparam name="TOld"></typeparam>
        /// <typeparam name="TNew"></typeparam>
        /// <param name="onProcess"></param>
        void UpdateStructureSet<TOld, TNew>(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
            where TOld : class
            where TNew : class;

        /// <summary>
        /// Creates a NON Transactional <see cref="IQueryEngine"/> used for searching.
        /// Is designed for being short lived. Create, consume and dispose.
        /// </summary>
        /// <returns></returns>
        IQueryEngine CreateQueryEngine();

        /// <summary>
        /// Creates a Transactional UnitOfWork <see cref="IUnitOfWork"/>, which is designed
        /// for being shortlived. Create, consume and dispose.
        /// </summary>
        /// <returns></returns>
        IUnitOfWork CreateUnitOfWork();
    }
}