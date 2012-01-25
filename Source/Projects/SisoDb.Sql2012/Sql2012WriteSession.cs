using SisoDb.Dac;
using SisoDb.Structures;

namespace SisoDb.Sql2012
{
    public class Sql2012WriteSession : DbWriteSession
    {
        protected internal Sql2012WriteSession(
			ISisoDbDatabase db,
			IDbClient dbClientTransactional,
			IDbClient dbClientNonTransactional,
			IIdentityStructureIdGenerator identityStructureIdGenerator)
            : base(db, dbClientTransactional, dbClientNonTransactional, identityStructureIdGenerator)
        {
        }
    }
}