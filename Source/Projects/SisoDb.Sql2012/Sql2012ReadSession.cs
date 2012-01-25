using SisoDb.Dac;

namespace SisoDb.Sql2012
{
    public class Sql2012ReadSession : DbReadSession
    {
        internal Sql2012ReadSession(ISisoDbDatabase db, IDbClient dbClient)
			: base(db, dbClient)
        {}
    }
}