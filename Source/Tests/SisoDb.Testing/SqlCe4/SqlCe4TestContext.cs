using SisoDb.SqlCe4;

namespace SisoDb.Testing.SqlCe4
{
    public class SqlCe4TestContext : TestContextBase
    {
        public SqlCe4TestContext(string connectionStringName)
            : base(connectionStringName.CreateSqlCe4Db())
        {
            DbHelper = new SqlCe4TestDbUtils(Database.ProviderFactory.GetAdoDriver(), Database.ConnectionInfo.ClientConnectionString);
        }
    }
}