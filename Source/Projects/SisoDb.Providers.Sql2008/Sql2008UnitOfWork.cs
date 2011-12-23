using SisoDb.Dac;
using SisoDb.Structures;

namespace SisoDb.Sql2008
{
    public class Sql2008UnitOfWork : DbUnitOfWork
    {
        protected internal Sql2008UnitOfWork(
			IDbDatabase db,
			IDbClient dbClientTransactional,
			IDbClient dbClientNonTransactional,
			IIdentityStructureIdGenerator identityStructureIdGenerator)
            : base(db, dbClientTransactional, dbClientNonTransactional, identityStructureIdGenerator)
        {
        }
    }
}