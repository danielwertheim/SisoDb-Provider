using SisoDb.Sql2012;

namespace SisoDb.Testing.Sql2012
{
    public class Sql2012TestContext : TestContextBase
    {
        public Sql2012TestContext(string connectionStringName)
            : base(new Sql2012DbFactory(), new Sql2012ConnectionInfo(connectionStringName), new Sql2012ProviderFactory())
        {
            DbHelper = new Sql2012TestDbUtils(Database.ConnectionInfo.ConnectionString.PlainString);
            DbHelperForServer = new Sql2012TestDbUtils(Database.ConnectionInfo.ServerConnectionString.PlainString);
        }
    }
}