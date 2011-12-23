using SisoDb.Dac;

namespace SisoDb.SqlCe4
{
    public class SqlCe4QueryEngine : DbQueryEngine
    {
        internal SqlCe4QueryEngine(IDbDatabase db, IDbClient dbClient)
            : base(db, dbClient)
        {}
    }
}