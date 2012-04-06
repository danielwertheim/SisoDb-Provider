using SisoDb.MiniProfiler;
using SisoDb.Sql2012;

namespace SisoDb.Testing.SqlProfiler
{
    public class SqlProfilerTestContext : TestContextBase
    {
        public SqlProfilerTestContext(string connectionStringName)
            : base(connectionStringName.CreateSql2012Db())
        {
            DbHelper = new SqlProfilerTestDbUtils(Database.ConnectionInfo.ClientConnectionString.PlainString);
            Database.ActivateProfiler();
        }
    }
}