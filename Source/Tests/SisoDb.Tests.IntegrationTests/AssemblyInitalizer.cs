using NUnit.Framework;
using SisoDb.Sql2008;
using SisoDb.SqlCe4;

namespace SisoDb.Tests.IntegrationTests
{
    [SetUpFixture]
    public class AssemblyInitalizer
    {
        [SetUp]
        public void Initialize()
        {
            EnsureSql2008DbExists();
        }

        private static void EnsureSql2008DbExists()
        {
            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSql2008);
            var database = new Sql2008DbFactory().CreateDatabase(connectionInfo);
            database.EnsureNewDatabase();
        }

        private static void EnsureSqlCe4DbExists()
        {
            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSqlCe4);
            var database = new SqlCe4DbFactory().CreateDatabase(connectionInfo);
            database.EnsureNewDatabase();
        }
    }
}