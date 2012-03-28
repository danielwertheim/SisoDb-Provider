using SisoDb.Testing.Sql2012;

namespace SisoDb.Testing.SqlProfiler
{
	public class SqlProfilerTestDbUtils : Sql2012TestDbUtils
    {
        public SqlProfilerTestDbUtils(string connectionString) : base(connectionString)
        {}
    }
}