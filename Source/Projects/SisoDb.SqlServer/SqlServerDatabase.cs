namespace SisoDb.SqlServer
{
    public abstract class SqlServerDatabase : SisoDatabase
    {
        protected SqlServerDatabase(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory) 
			: base(connectionInfo, dbProviderFactory)
        {
        }
    }
}