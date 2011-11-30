using SisoDb.DbSchema;

namespace SisoDb.Sql2008
{
    public class Sql2008QueryEngine : DbQueryEngine
    {
        internal Sql2008QueryEngine(
            ISisoDatabase db,
            IDbSchemaManager dbSchemaManager)
            : base(db, dbSchemaManager, false)
        {}

        protected Sql2008QueryEngine(
            ISisoDatabase db,
            IDbSchemaManager dbSchemaManager,
            bool transactional)
            : base(db, dbSchemaManager, transactional)
        {
        }
    }
}