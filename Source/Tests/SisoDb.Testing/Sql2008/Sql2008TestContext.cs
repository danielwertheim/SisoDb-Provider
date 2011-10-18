using SisoDb.Sql2008;

namespace SisoDb.Testing.Sql2008
{
    public class Sql2008TestContext : TestContextBase
    {
        public Sql2008TestContext(string connectionStringName)
            : base(new Sql2008DbFactory(), new Sql2008ConnectionInfo(connectionStringName), new Sql2008ProviderFactory())
        {
            DbHelper = new Sql2008TestDbUtils(Database.ConnectionInfo.ConnectionString.PlainString);
            DbHelperForServer = new Sql2008TestDbUtils(Database.ConnectionInfo.ServerConnectionString.PlainString);
        }
    }
}