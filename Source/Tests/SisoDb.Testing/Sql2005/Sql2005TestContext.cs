using SisoDb.Sql2005;

namespace SisoDb.Testing.Sql2005
{
    public class Sql2005TestContext : TestContextBase
    {
        public Sql2005TestContext(string connectionStringName)
            : base(connectionStringName.CreateSql2005Db())
        {
            DbHelper = new Sql2005TestDbUtils(Database.ProviderFactory.GetAdoDriver(), Database.ConnectionInfo.ClientConnectionString);
        }
    }
}