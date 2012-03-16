using System.Linq;

namespace SisoDb.Testing
{
    public abstract class TestContextBase : ITestContext
    {
        public ISisoDatabase Database { get; private set; }
        public IDbProviderFactory ProviderFactory { get; private set; }
        public ITestDbUtils DbHelper { get; protected set; }

        protected TestContextBase(ISisoDatabaseFactory dbFactory, ISisoConnectionInfo connectionInfo, IDbProviderFactory providerFactory)
        {
            Database = dbFactory.CreateDatabase(connectionInfo);
            ProviderFactory = providerFactory;
        }

        public void Cleanup()
        {
			if (Database != null)
			{
				Database.Maintenance.Clear();
				Database = null;
			}

            if(ProviderFactory != null)
            {
                ProviderFactory.ConnectionManager.ReleaseAllDbConnections();
            }
        }
    }
}