using SisoDb.Dac;

namespace SisoDb.Sql2008
{
    public class Sql2008ReadSession : DbReadSession
    {
        internal Sql2008ReadSession(ISisoDbDatabase db, IDbClient dbClient)
			: base(db, dbClient)
        {}
    }
}