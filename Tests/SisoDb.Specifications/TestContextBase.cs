using SisoDb.Specifications.Sql2008;

namespace SisoDb.Specifications
{
    public abstract class TestContextBase : ITestContext
    {
        public ITestDbUtils DbHelper { get; private set; }
        public ITestDbUtils DbHelperForServer { get; private set; }
        public ISisoDatabase Database { get; private set; }
        
        protected TestContextBase(ISisoDbFactory dbFactory, ISisoConnectionInfo connectionInfo)
        {
            Database = dbFactory.CreateDatabase(connectionInfo);
            DbHelper = new Sql2008TestDbUtils(Database.ConnectionInfo.ConnectionString.PlainString);
            DbHelperForServer = new Sql2008TestDbUtils(Database.ConnectionInfo.ServerConnectionString.PlainString);
        }

        public void Cleanup()
        {
            Database.DropStructureSets();
        }
    }
}