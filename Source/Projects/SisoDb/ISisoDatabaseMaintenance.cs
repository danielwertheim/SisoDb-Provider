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
    }
}