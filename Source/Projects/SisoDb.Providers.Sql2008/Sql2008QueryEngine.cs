using SisoDb.Dac;

namespace SisoDb.Sql2008
{
    public class Sql2008QueryEngine : DbQueryEngine
    {
        internal Sql2008QueryEngine(IDbDatabase db, IDbClient dbClient)
			: base(db, dbClient)
        {}
    }
}