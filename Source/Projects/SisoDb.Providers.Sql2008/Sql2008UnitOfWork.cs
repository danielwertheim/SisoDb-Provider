using SisoDb.DbSchema;

namespace SisoDb.Sql2008
{
    public class Sql2008UnitOfWork : DbUnitOfWork
    {
        protected internal Sql2008UnitOfWork(
            ISisoDatabase db,
            IDbSchemaManager dbSchemaManager)
            : base(db, dbSchemaManager)
        {
        }
    }
}