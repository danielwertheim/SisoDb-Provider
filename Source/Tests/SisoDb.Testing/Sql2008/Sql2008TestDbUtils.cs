using SisoDb.Dac;

namespace SisoDb.Testing.Sql2008
{
    public class Sql2008TestDbUtils : SqlServerTestDbUtils
    {
        public Sql2008TestDbUtils(IAdoDriver driver, string connectionString) : base(driver, connectionString)
        {
        }
    }
}