using SisoDb.Dac;
using SisoDb.DbSchema;

namespace SisoDb.SqlCe4
{
    public class SqlCe4ReadSession : DbReadSession
    {
        internal SqlCe4ReadSession(ISisoDatabase db, IDbClient dbClient, IDbSchemaManager dbSchemaManager)
            : base(db, dbClient, dbSchemaManager)
        {}
    }
}