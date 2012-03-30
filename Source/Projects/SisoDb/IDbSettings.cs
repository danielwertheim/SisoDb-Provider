namespace SisoDb
{
    public interface IDbSettings 
    {
        /// <summary>
        /// Controls the maximum size of each batch used for inserting many structures.
        /// </summary>
        int MaxInsertManyBatchSize { get; set; }
        
        /// <summary>
        /// Controls the maximum size of each batch used for updating many structures.
        /// </summary>
        int MaxUpdateManyBatchSize { get; set; }

        /// <summary>
        /// Controls if a synchronization between your code model and query-index tables
        /// should be performed (true) or not (false).
        /// If (true), per <see cref="ISisoDatabase"/>, schemas are synhronized once. Hence
        /// a compare between your code model, e.g c-sharp class Person and
        /// the keys (memberpaths) in each query index table for Person is synchronized.
        /// </summary>
        bool SynchronizeSchemaChanges { get; set; }
    }
}