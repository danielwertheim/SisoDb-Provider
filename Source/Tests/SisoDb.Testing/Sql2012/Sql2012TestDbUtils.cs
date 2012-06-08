using SisoDb.Dac;

namespace SisoDb.Testing.Sql2012
{
    public class Sql2012TestDbUtils : SqlServerTestDbUtils
    {
        public Sql2012TestDbUtils(IAdoDriver driver, string connectionString) : base(driver, connectionString)
        {
        }
    }
}