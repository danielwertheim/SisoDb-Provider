namespace SisoDb.DbSchema
{
    public enum SchemaChanges
    {
        /// <summary>
        /// Exists in the structure-schema but not in db,
        /// hence it should be added to the db.
        /// </summary>
        IsMissingColumn,

        /// <summary>
        /// Exists in db but not in structure-schema, hence it
        /// should be removed from the database.
        /// </summary>
        IsObsoleteColumn
    }
}