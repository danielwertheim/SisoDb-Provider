using SisoDb.Sql2008;

namespace SisoDb.Testing.Sql2008
{
    public class Sql2008TestContext : TestContextBase
    {
        public Sql2008TestContext(string connectionStringName)
            : base(connectionStringName.CreateSql2008Db())
        {
            DbHelper = new Sql2008TestDbUtils(Database.ProviderFactory.GetAdoDriver(), Database.ConnectionInfo.ClientConnectionString);
        }
    }
}