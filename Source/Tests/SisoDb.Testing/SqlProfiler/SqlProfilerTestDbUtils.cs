using SisoDb.Dac;
using SisoDb.Testing.Sql2012;

namespace SisoDb.Testing.SqlProfiler
{
	public class SqlProfilerTestDbUtils : Sql2012TestDbUtils
    {
        public SqlProfilerTestDbUtils(IAdoDriver driver, string connectionString) 
            : base(driver, connectionString)
        {}
    }
}