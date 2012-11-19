namespace SisoDb.Testing
{
    public abstract class TestContextBase : ITestContext
    {
        public ISisoDatabase Database { get; private set; }
        public ITestDbUtils DbHelper { get; protected set; }

        protected TestContextBase(ISisoDatabase db)
        {
            Database = db;
        }

        public void Cleanup()
        {
			if (Database != null)
			{
                if(Database.CacheProvider != null)
                    Database.CacheProvider.Clear();
				Database.Maintenance.Reset();
                Database.ProviderFactory.ConnectionManager.ReleaseAllConnections();
				Database = null;
			}
        }
    }
}