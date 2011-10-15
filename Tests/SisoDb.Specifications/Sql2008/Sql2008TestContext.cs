using SisoDb.Sql2008;

namespace SisoDb.Specifications.Sql2008
{
    public class Sql2008TestContext : TestContextBase
    {
        public Sql2008TestContext(string connectionStringName)
            : base(new Sql2008DbFactory(), new Sql2008ConnectionInfo(connectionStringName))
        {
        }
    }
}