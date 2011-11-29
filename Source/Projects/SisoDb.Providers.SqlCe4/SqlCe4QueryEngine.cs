using SisoDb.DbSchema;

namespace SisoDb.SqlCe4
{
    public class SqlCe4QueryEngine : DbQueryEngine
    {
        internal SqlCe4QueryEngine(
            ISisoDatabase db,
            IDbSchemaManager dbSchemaManager)
            : base(db, dbSchemaManager, false)
        {}

        protected SqlCe4QueryEngine(
            ISisoDatabase db,
            IDbSchemaManager dbSchemaManager,
            bool transactional)
            : base(db, dbSchemaManager, transactional)
        {
        }
    }
}