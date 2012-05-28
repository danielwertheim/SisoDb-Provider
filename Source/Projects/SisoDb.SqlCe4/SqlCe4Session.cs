using SisoDb.SqlServer;

namespace SisoDb.SqlCe4
{
    public class SqlCe4Session : SqlServerSession
    {
        internal SqlCe4Session(ISisoDatabase db) : base(db)
        {}
    }
}