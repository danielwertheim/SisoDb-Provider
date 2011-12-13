using SisoDb.Dac;
using SisoDb.DbSchema;

namespace SisoDb.SqlCe4
{
    public class SqlCe4UnitOfWork : DbUnitOfWork
    {
        protected internal SqlCe4UnitOfWork(ISisoDatabase db, IDbClient dbClient, IDbSchemaManager dbSchemaManager)
            : base(db, dbClient, dbSchemaManager)
        {
        }
    }
}