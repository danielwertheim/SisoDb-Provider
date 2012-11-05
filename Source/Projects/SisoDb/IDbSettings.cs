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
        /// DEFAULT IS: True.
        /// Controls if upserting of structureset schemas should be allowed or not.
        /// If (true), then ONCE per <see cref="ISisoDatabase"/>, table-schemas are created on the fly.
        /// If (false), then the structureset allready has to be created before.
        /// This is useful when you have deployed and settled upon your design. If you then set it to (false),
        /// you will get a small performance boost, since it will not check if tables exists nor will it create
        /// any new once on the fly the first time you use a certain structure.
        /// </summary>
        bool AllowDynamicSchemaCreation { get; set; }

        /// <summary>
        /// DEFAULT IS: True.
        /// Controls if a synchronization between your code model and query-index tables
        /// should be performed (true) or not (false).
        /// If (true), then ONCE per <see cref="ISisoDatabase"/>, schemas are synhronized on the fly. Hence
        /// a compare between your code model, e.g c-sharp class Person and
        /// the keys (memberpaths) in each query index table for Person is synchronized.
        /// </summary>
        bool AllowDynamicSchemaUpdates { get; set; }
    }
}