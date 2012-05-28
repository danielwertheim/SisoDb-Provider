namespace SisoDb.SqlServer
{
    public abstract class SqlServerSession : DbSession
    {
        protected SqlServerSession(ISisoDatabase db)
			: base(db)
        {}
    }
}