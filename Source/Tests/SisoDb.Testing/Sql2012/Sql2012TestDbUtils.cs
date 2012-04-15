using SisoDb.Dac;
using SisoDb.Testing.Sql2008;

namespace SisoDb.Testing.Sql2012
{
	public class Sql2012TestDbUtils : Sql2008TestDbUtils
    {
        public Sql2012TestDbUtils(IAdoDriver driver, string connectionString) 
            : base(driver, connectionString)
        {}
    }
}