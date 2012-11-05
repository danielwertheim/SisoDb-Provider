using SisoDb.Sql2012;

namespace SisoDb.Testing.Sql2012
{
    public class Sql2012TestContext : TestContextBase
    {
        public Sql2012TestContext(string connectionStringName)
            : base(connectionStringName.CreateSql2012Db())
        {
            DbHelper = new Sql2012TestDbUtils(Database.ProviderFactory.GetAdoDriver(), Database.ConnectionInfo.ClientConnectionString);
        }
    }
}