namespace SisoDb.Sql2012
{
    public class Sql2012Database : SisoDatabase
    {
    	public Sql2012Database(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory) 
			: base(connectionInfo, dbProviderFactory)
        {
        }

        protected override DbSession CreateSession()
        {
            return new Sql2012Session(this);
        }
    }
}