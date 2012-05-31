using SisoDb.SqlServer;

namespace SisoDb.Sql2008
{
    public class Sql2008Session : SqlServerSession
    {
        internal Sql2008Session(ISisoDatabase db) : base(db)
        {}
    }
}