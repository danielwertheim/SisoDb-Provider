using SisoDb.SqlServer;

namespace SisoDb.Sql2012
{
    public class Sql2012Session : SqlServerSession
    {
        internal Sql2012Session(ISisoDatabase db) : base(db)
        {}
    }
}