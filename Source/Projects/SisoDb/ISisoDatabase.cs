using System;
using SisoDb.DbSchema;
using SisoDb.Serialization;
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
        /// Lock object used to synchronize work against Db-operations.
        /// </summary>
        object LockObject { get; }

        /// <summary>
        /// The name of the database.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Connection info for the database.
        /// </summary>
        ISisoConnectionInfo ConnectionInfo { get; }

        /// <summary>
        /// Provider factory.
        /// </summary>
        IDbProviderFactory ProviderFactory { get; }

        /// <summary>
        /// Runtime settings associated with the db.
        /// </summary>
        IDbSettings Settings { get; set; }

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
        /// Caches and controls state of Db-Schemas.
        /// </summary>
        IDbSchemas DbSchemas { get; }

        /// <summary>
        /// Structure builders collection used to resolve a Structure builder to use when building structures for insert and updates.
        /// </summary>
        IStructureBuilders StructureBuilders { get; set; }

        /// <summary>
        /// The serializer used to handle Json.
        /// </summary>
        ISisoSerializer Serializer { get; set; }

        /// <summary>
        /// Used for maintenance tasks of the database.
        /// E.g Regeneration of query indexes and Migrations.
        /// </summary>
        ISisoDatabaseMaintenance Maintenance { get; }

        /// <summary>
        /// Ensures that a new fresh database will exists. Drops any existing database.
        /// </summary>
        ISisoDatabase EnsureNewDatabase();

        /// <summary>
        /// Creates and initializes a new database if one does not exist.
        /// </summary>
        ISisoDatabase CreateIfNotExists();

        /// <summary>
        /// Initializes an existing database by creating Siso-system tables. 
        /// </summary>
        ISisoDatabase InitializeExisting();

        /// <summary>
        /// Deletes the databse if it exists.
        /// </summary>
        ISisoDatabase DeleteIfExists();

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
        ISisoDatabase DropStructureSet<T>() where T : class;

        /// <summary>
        /// Drops the structure set, meaning all tables associated with
        /// the structure type.
        /// </summary>
        /// <param name="type"></param>
        ISisoDatabase DropStructureSet(Type type);

        /// <summary>
        /// Drops ALL structure sets for sent <paramref name="types"/>.
        /// </summary>
        /// <param name="types"></param>
        ISisoDatabase DropStructureSets(Type[] types);

        /// <summary>
        /// Manually upserts a structure set, meaning all tables for
        /// the structure type <typeparamref name="T"/> will be created.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        ISisoDatabase UpsertStructureSet<T>() where T : class;

        /// <summary>
        /// Manually upserts a structure set, meaning all tables for
        /// the structure type <paramref name="type"/> will be created.
        /// </summary>
        /// <param name="type"></param>
        ISisoDatabase UpsertStructureSet(Type type);

        /// <summary>
        /// Manually upserts structre sets, meaning all tables for
        /// sent stucture types <paramref name="types"/> will be created.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        ISisoDatabase UpsertStructureSet(Type[] types);

        /// <summary>
		/// Creates an <see cref="ISession"/>, used for inserts, updates, deletes and searching.
        /// The Session is designed for being short lived. Create, consume and dispose.
        /// </summary>
        /// <returns></returns>
        ISession BeginSession();

        /// <summary>
        /// Opens a new Session using <see cref="BeginSession"/> and passes that to your consuming closure.
        /// </summary>
        /// <param name="consumer"></param>
        void WithSession(Action<ISession> consumer);
        
        /// <summary>
        /// Opens a new Session using <see cref="BeginSession"/> and passes that to your consuming closure that
        /// returns <typeparamref name="T"></typeparamref>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="consumer"></param>
        /// <returns></returns>
        T WithSession<T>(Func<ISession, T> consumer);

        /// <summary>
        /// Use when you want to execute a single search operation against the <see cref="ISisoDatabase"/>
		/// via an <see cref="ISession"/>.
        /// </summary>
        /// <returns></returns>
        /// <remarks>If you need to do multiple queries, inserts etc, then use <see cref="BeginSession"/> instead.</remarks>
        ISingleOperationSession UseOnceTo();
    }
}