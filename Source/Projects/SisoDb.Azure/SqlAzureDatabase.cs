namespace SisoDb.Azure
{
    public class SqlAzureDatabase : SisoDatabase
    {
    	public SqlAzureDatabase(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory) 
			: base(connectionInfo, dbProviderFactory)
        {
        }

        protected override DbSession CreateSession()
        {
            return new SqlAzureSession(this);
        }
    }
}