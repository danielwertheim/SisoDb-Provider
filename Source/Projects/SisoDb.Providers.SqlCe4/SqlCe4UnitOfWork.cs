using SisoDb.DbSchema;

namespace SisoDb.SqlCe4
{
    public class SqlCe4UnitOfWork : DbUnitOfWork
    {
        protected internal SqlCe4UnitOfWork(
            ISisoDatabase db,
            IDbSchemaManager dbSchemaManager)
            : base(db, dbSchemaManager)
        {
        }
    }
}