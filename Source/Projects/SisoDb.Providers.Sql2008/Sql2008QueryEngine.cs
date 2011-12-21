using SisoDb.Dac;
using SisoDb.DbSchema;

namespace SisoDb.Sql2008
{
    public class Sql2008QueryEngine : DbQueryEngine
    {
        internal Sql2008QueryEngine(ISisoDatabase db, IDbClient dbClient, IDbSchemaManager dbSchemaManager, ISqlStatements sqlStatements)
			: base(db, dbClient, dbSchemaManager, sqlStatements)
        {}
    }
}