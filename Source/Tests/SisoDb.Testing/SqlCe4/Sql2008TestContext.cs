using SisoDb.Sql2008;

namespace SisoDb.Testing.SqlCe4
{
    public class SqlCe4TestContext : TestContextBase
    {
        public SqlCe4TestContext(string connectionStringName)
            : base(new Sql2008DbFactory(), new Sql2008ConnectionInfo(connectionStringName), new Sql2008ProviderFactory())
        {
            DbHelper = new SqlCe4TestDbUtils(Database.ConnectionInfo.ConnectionString.PlainString);
            DbHelperForServer = new SqlCe4TestDbUtils(Database.ConnectionInfo.ServerConnectionString.PlainString);
        }
    }
}