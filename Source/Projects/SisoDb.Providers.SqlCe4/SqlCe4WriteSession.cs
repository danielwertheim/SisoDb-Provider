using SisoDb.Dac;
using SisoDb.Structures;

namespace SisoDb.SqlCe4
{
    public class SqlCe4WriteSession : DbWriteSession
    {
		protected internal SqlCe4WriteSession(
			ISisoDbDatabase db, 
			IDbClient dbClientTransactional, 
			IDbClient dbClientNonTransactional, 
			IIdentityStructureIdGenerator identityStructureIdGenerator)
            : base(db, dbClientTransactional, dbClientNonTransactional, identityStructureIdGenerator)
        {
        }
    }
}