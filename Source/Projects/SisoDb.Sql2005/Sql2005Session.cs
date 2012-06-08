using SisoDb.SqlServer;

namespace SisoDb.Sql2005
{
    public class Sql2005Session : SqlServerSession
    {
        internal Sql2005Session(ISisoDatabase db) : base(db)
        {}
    }
}