using System.Linq;

namespace SisoDb.Testing
{
    public abstract class TestContextBase : ITestContext
    {
        public ISisoDatabase Database { get; private set; }
        protected IDbProviderFactory ProviderFactory { get; private set; }
        public ITestDbUtils DbHelper { get; protected set; }
        public ITestDbUtils DbHelperForServer { get; protected set; }

        protected TestContextBase(ISisoDbFactory dbFactory, ISisoConnectionInfo connectionInfo, IDbProviderFactory providerFactory)
        {
            Database = dbFactory.CreateDatabase(connectionInfo);
            ProviderFactory = providerFactory;
        }

        public void Cleanup()
        {
			if (Database != null)
			{
				Database.DropStructureSets(Database.StructureSchemas.GetRegistrations().Select(r => r.Key).ToArray());
				Database = null;
			}
        }
    }
}