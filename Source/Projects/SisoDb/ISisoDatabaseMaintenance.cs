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
        /// Regenerates key-values used for querying.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        void RegenerateQueryIndexesFor<TContract, TImpl>()
            where TContract : class
            where TImpl : class;

        /// <summary>
        /// Lets you migrate all structures from <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>.
        /// You can also control if a certain structure should be dumped by returning <see cref="MigrationStatuses.Trash"/>.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="modifier"></param>
        void Migrate<TFrom, TTo>(Func<TFrom, TTo, MigrationStatuses> modifier)
            where TFrom : class
            where TTo : class;

        /// <summary>
        /// Lets you migrate all structures from <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>.
        /// You can also control if a certain structure should be dumped by returning <see cref="MigrationStatuses.Trash"/>.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="migration"></param>
        void Migrate<TFrom, TTo>(Migration<TFrom, TTo> migration)
            where TFrom : class
            where TTo : class;

        /// <summary>
        /// Lets you migrate all structures from <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>.
        /// Useful when targetting the same table and you only have one class for the model, hence <typeparamref name="TFrom"/>
        /// and <typeparamref name="TTo"/> are the same. Then you can use a custom map type (anonymous or static) as <typeparamref name="TFromTemplate"/>.
        /// You can also control if a certain structure should be dumped by returning <see cref="MigrationStatuses.Trash"/>.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TFromTemplate"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="migration"></param>
        void Migrate<TFrom, TFromTemplate, TTo>(Migration<TFrom, TFromTemplate, TTo> migration)
            where TFrom : class
            where TFromTemplate : class
            where TTo : class;
    }
}