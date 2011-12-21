using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;

namespace SisoDb.SqlCe4
{
    public class SqlCe4QueryEngine : DbQueryEngine
    {
        internal SqlCe4QueryEngine(ISisoDatabase db, IDbClient dbClient, IDbSchemaManager dbSchemaManager, ISqlStatements sqlStatements)
            : base(db, dbClient, dbSchemaManager, sqlStatements)
        {}
    }
}