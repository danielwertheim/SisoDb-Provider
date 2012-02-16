using System;
using PineCone.Structures.Schemas;
using SisoDb.Serialization;
using SisoDb.Structures;

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
        /// Connection info for the database.
        /// </summary>
        ISisoConnectionInfo ConnectionInfo { get; }

		/// <summary>
		/// By assigning a <see cref="ICacheProvider"/> you get
		/// the possibility of preventing the query from
		/// hitting the database for certain queries.
		/// </summary>
		ICacheProvider CacheProvider { get; set; }

		/// <summary>
		/// Get a value indicating if the Database has a <see cref="CacheProvider"/>
		/// assigned.
		/// </summary>
		bool CachingIsEnabled { get; }

        /// <summary>
		/// Cached Structure schemas, which holds information
		/// about members to index etc.
        /// </summary>
        IStructureSchemas StructureSchemas { get; set; }

        /// <summary>
        /// Structure builders collection used to resolve a Structure builder to use when building structures for insert and updates.
        /// </summary>
        IStructureBuilders StructureBuilders { get; set; }

        /// <summary>
        /// The serializer used to handle Json.
        /// </summary>
        IJsonSerializer Serializer { get; set; }

		/// <summary>
		/// Returns an <see cref="IStructureSetMigrator"/> used for
		/// assisting you with model migrations.
		/// </summary>
		/// <returns></returns>
    	IStructureSetMigrator GetStructureSetMigrator();

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
        /// Drops the structure set, meaning all tables associated with
        /// the structure type.
        /// </summary>
        /// <param name="type"></param>
        void DropStructureSet(Type type);

        /// <summary>
        /// Drops ALL structure sets.
        /// </summary>
        /// <param name="types"></param>
        void DropStructureSets(Type[] types);

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
        /// Manually upserts a structure set, meaning all tables for
        /// the structure type will be created.
        /// </summary>
        /// <param name="type"></param>
        void UpsertStructureSet(Type type);

        /// <summary>
		/// Creates a NON Transactional <see cref="ISession"/>, used for searching.
        /// The Session is designed for being short lived. Create, consume and dispose.
        /// </summary>
        /// <returns></returns>
        ISession BeginSession();

        /// <summary>
        /// Use when you want to execute a single search operation against the <see cref="ISisoDatabase"/>
		/// via an <see cref="ISession"/>.
        /// </summary>
        /// <returns></returns>
        /// <remarks>If you need to do multiple queries, inserts etc, then use <see cref="BeginSession"/> instead.</remarks>
        ISingleOperationSession UseOnceTo();
    }
}