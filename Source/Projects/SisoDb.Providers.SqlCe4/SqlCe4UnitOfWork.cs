using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Structures;

namespace SisoDb.SqlCe4
{
    public class SqlCe4UnitOfWork : DbUnitOfWork
    {
		protected internal SqlCe4UnitOfWork(ISisoDatabase db, IDbClient dbClientTransactional, IDbClient dbClientNonTransactional, IDbSchemaManager dbSchemaManager, IIdentityStructureIdGenerator identityStructureIdGenerator)
            : base(db, dbClientTransactional, dbClientNonTransactional, dbSchemaManager, identityStructureIdGenerator)
        {
        }
    }
}