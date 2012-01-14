using SisoDb.Dac;
using SisoDb.Structures;

namespace SisoDb.Sql2008
{
    public class Sql2008WriteSession : DbWriteSession
    {
        protected internal Sql2008WriteSession(
			ISisoDbDatabase db,
			IDbClient dbClientTransactional,
			IDbClient dbClientNonTransactional,
			IIdentityStructureIdGenerator identityStructureIdGenerator)
            : base(db, dbClientTransactional, dbClientNonTransactional, identityStructureIdGenerator)
        {
        }
    }
}