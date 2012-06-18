using SisoDb.SqlServer;

namespace SisoDb.Sql2005
{
    public class Sql2005Database : SqlServerDatabase
    {
        public Sql2005Database(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory) 
			: base(connectionInfo, dbProviderFactory)
        {
        }

    	protected override DbSession CreateSession()
    	{
    	    return new Sql2005Session(this);
    	}
    }
}