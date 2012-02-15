using SisoDb.SqlCe4;

namespace SisoDb.Testing.SqlCe4
{
    public class SqlCe4TestContext : TestContextBase
    {
        public SqlCe4TestContext(string connectionStringName)
            : base(new SqlCe4DbFactory(), new SqlCe4ConnectionInfo(connectionStringName), new SqlCe4ProviderFactory())
        {
            DbHelper = new SqlCe4TestDbUtils(Database.ConnectionInfo.ClientConnectionString);
        }
    }
}