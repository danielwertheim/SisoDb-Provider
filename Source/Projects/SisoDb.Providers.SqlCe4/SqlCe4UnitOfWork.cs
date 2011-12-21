using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Structures;

namespace SisoDb.SqlCe4
{
    public class SqlCe4UnitOfWork : DbUnitOfWork
    {
		protected internal SqlCe4UnitOfWork(
			ISisoDatabase db, 
			IDbClient dbClientTransactional, 
			IDbClient dbClientNonTransactional, 
			IDbSchemaManager dbSchemaManager, 
			IIdentityStructureIdGenerator identityStructureIdGenerator,
			ISqlStatements sqlStatements)
            : base(db, dbClientTransactional, dbClientNonTransactional, dbSchemaManager, identityStructureIdGenerator, sqlStatements)
        {
        }
    }
}