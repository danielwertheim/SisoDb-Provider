using System;
using System.Diagnostics;
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
		/// Creates a NON Transactional <see cref="IReadSession"/>, used for searching.
        /// The Session is designed for being short lived. Create, consume and dispose.
        /// </summary>
        /// <returns></returns>
        IReadSession BeginReadSession();

        /// <summary>
        /// Creates a Transactional <see cref="IWriteSession"/>, used for writing or searching.
		/// The Session is designed for being short lived. Create, consume and dispose.
        /// </summary>
        /// <returns></returns>
        IWriteSession BeginWriteSession();

        /// <summary>
        /// Use when you want to execute a single search operation against the <see cref="ISisoDatabase"/>
		/// via an <see cref="IReadSession"/>.
        /// </summary>
        /// <returns></returns>
        /// <remarks>If you need to do multiple queries, use <see cref="BeginReadSession"/> instead.</remarks>
        [DebuggerStepThrough]
        IReadOnce ReadOnce();

        /// <summary>
        /// Use when you want to execute a single Insert, Update or Delete against 
        /// the <see cref="ISisoDatabase"/> via an <see cref="IWriteSession"/>.
        /// </summary>
        /// <returns></returns>
        /// <remarks>If you need to do multiple operations in the <see cref="IWriteSession"/>,
        /// use <see cref="BeginWriteSession"/> instead.</remarks>
        [DebuggerStepThrough]
        IWriteOnce WriteOnce();

        /// <summary>
        /// Simplifies usage of <see cref="IWriteSession"/>.
        /// </summary>
        /// <param name="consumer"></param>
        [DebuggerStepThrough]
        void WithWriteSession(Action<IWriteSession> consumer);

        /// <summary>
		/// Simplifies usage of <see cref="IReadSession"/>.
        /// </summary>
        /// <param name="consumer"></param>
        [DebuggerStepThrough]
		void WithReadSession(Action<IReadSession> consumer);

		/// <summary>
		/// Simplifies usage of <see cref="IReadSession"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="consumer"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		T WithReadSession<T>(Func<IReadSession, T> consumer);
    }
}