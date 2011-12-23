using SisoDb.Dac;
using SisoDb.Structures;

namespace SisoDb.SqlCe4
{
    public class SqlCe4UnitOfWork : DbUnitOfWork
    {
		protected internal SqlCe4UnitOfWork(
			IDbDatabase db, 
			IDbClient dbClientTransactional, 
			IDbClient dbClientNonTransactional, 
			IIdentityStructureIdGenerator identityStructureIdGenerator)
            : base(db, dbClientTransactional, dbClientNonTransactional, identityStructureIdGenerator)
        {
        }
    }
}