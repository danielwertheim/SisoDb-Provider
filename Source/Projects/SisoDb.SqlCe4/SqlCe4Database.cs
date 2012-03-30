namespace SisoDb.SqlCe4
{
    public class SqlCe4Database : SisoDatabase
    {
    	public SqlCe4Database(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory)
            : base(connectionInfo, dbProviderFactory)
        {
        }

        protected override DbSession CreateSession()
        {
            return new SqlCe4Session(this);
        }
    }
}