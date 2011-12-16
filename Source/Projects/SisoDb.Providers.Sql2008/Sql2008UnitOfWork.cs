using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Structures;

namespace SisoDb.Sql2008
{
    public class Sql2008UnitOfWork : DbUnitOfWork
    {
        protected internal Sql2008UnitOfWork(ISisoDatabase db, IDbClient dbClientTransactional, IDbClient dbClientNonTransactional, IDbSchemaManager dbSchemaManager, IIdentityStructureIdGenerator identityStructureIdGenerator)
            : base(db, dbClientTransactional, dbClientNonTransactional, dbSchemaManager, identityStructureIdGenerator)
        {
        }
    }
}