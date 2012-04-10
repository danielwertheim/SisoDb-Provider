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
				Database.Maintenance.Reset();
                Database.ProviderFactory.ConnectionManager.ReleaseAllDbConnections();
				Database = null;
			}
        }
    }
}