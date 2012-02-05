namespace SisoDb.SqlCe4
{
    public class SqlCe4Session : DbSession
    {
        internal SqlCe4Session(ISisoDbDatabase db) : base(db)
        {}
    }
}