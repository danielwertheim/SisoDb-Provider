using SisoDb.Dac;
using SisoDb.DbSchema;

namespace SisoDb.SqlCe4
{
    public class SqlCe4QueryEngine : DbQueryEngine
    {
        internal SqlCe4QueryEngine(ISisoDatabase db, IDbClient dbClient, IDbSchemaManager dbSchemaManager)
            : base(db, dbClient, dbSchemaManager)
        {}
    }
}