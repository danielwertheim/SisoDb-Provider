namespace SisoDb.Sql2008
{
    public class Sql2008Database : SisoDatabase
    {
        public Sql2008Database(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory) 
			: base(connectionInfo, dbProviderFactory)
        {
        }

    	protected override DbSession CreateSession()
    	{
    	    return new Sql2008Session(this);
    	}
    }
}