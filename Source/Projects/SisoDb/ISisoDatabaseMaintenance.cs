namespace SisoDb
{
    public interface ISisoDatabaseMaintenance
    {
        /// <summary>
        /// Drops all structure sets.
        /// </summary>
        void Clear();
    }
}