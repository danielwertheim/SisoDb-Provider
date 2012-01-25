using SisoDb.Dac;

namespace SisoDb.SqlCe4
{
    public class SqlCe4ReadSession : DbReadSession
    {
        internal SqlCe4ReadSession(ISisoDbDatabase db, IDbClient dbClient)
            : base(db, dbClient)
        {}
    }
}