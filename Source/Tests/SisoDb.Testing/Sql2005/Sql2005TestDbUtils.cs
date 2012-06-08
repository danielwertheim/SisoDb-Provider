using SisoDb.Dac;

namespace SisoDb.Testing.Sql2005
{
    public class Sql2005TestDbUtils : SqlServerTestDbUtils
    {
        public Sql2005TestDbUtils(IAdoDriver driver, string connectionString) : base(driver, connectionString)
        {
        }
    }
}