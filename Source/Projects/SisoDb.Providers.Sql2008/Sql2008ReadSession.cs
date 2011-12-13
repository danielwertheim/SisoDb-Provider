using SisoDb.Dac;
using SisoDb.DbSchema;

namespace SisoDb.Sql2008
{
    public class Sql2008ReadSession : DbReadSession
    {
        internal Sql2008ReadSession(ISisoDatabase db, IDbClient dbClient, IDbSchemaManager dbSchemaManager)
			: base(db, dbClient, dbSchemaManager)
        {}
    }
}