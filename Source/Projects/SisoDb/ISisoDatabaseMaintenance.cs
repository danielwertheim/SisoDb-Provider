using System;
using SisoDb.Maintenance;

namespace SisoDb
{
    public interface ISisoDatabaseMaintenance
    {
        /// <summary>
        /// Drops all structure sets and removes entries from SisoDb system tables.
        /// </summary>
        void Reset();

        /// <summary>
        /// Renames a structure.
        /// </summary>
        /// <param name="from">The old name, e.g Person</param>
        /// <param name="to">The new name, e.g People</param>
        void RenameStructure(string from, string to);

        /// <summary>
        /// Regenerates key-values used for querying.
        /// </summary>
        void RegenerateQueryIndexesFor<T>() where T : class;

        /// <summary>
        /// Lets you migrate all structures from <typeparamref name="TOld"/> to <typeparamref name="TNew"/>.
        /// You can also control if a certain structure should be dumped by returning <see cref="MigrationStatuses.Trash"/>.
        /// </summary>
        /// <typeparam name="TOld"></typeparam>
        /// <typeparam name="TNew"></typeparam>
        /// <param name="modifier"></param>
        void Migrate<TOld, TNew>(Func<TOld, TNew, MigrationStatuses> modifier) where TOld : class where TNew : class;
    }
}