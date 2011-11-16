using SisoDb.Providers;

namespace SisoDb.Testing
{
    public abstract class TestContextBase : ITestContext
    {
        public ISisoDatabase Database { get; private set; }
        protected ISisoProviderFactory ProviderFactory { get; private set; }
        public ITestDbUtils DbHelper { get; protected set; }
        public ITestDbUtils DbHelperForServer { get; protected set; }

        protected TestContextBase(ISisoDbFactory dbFactory, ISisoConnectionInfo connectionInfo, ISisoProviderFactory providerFactory)
        {
            Database = dbFactory.CreateDatabase(connectionInfo);
            ProviderFactory = providerFactory;
        }

        public void Cleanup()
        {
            Database.DropStructureSets();
        }
    }
}