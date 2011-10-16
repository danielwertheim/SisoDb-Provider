using Machine.Specifications;
using SisoDb.Sql2008;
using SisoDb.Testing;

namespace SisoDb.IntegrationTests
{
    public class AssemblyInitializer : IAssemblyContext
    {
        private static bool _isInitialized = false;

        public void OnAssemblyStart()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            EnsureSql2008DbExists();
            EnsureSqlCe4DbExists();
        }

        public void OnAssemblyComplete()
        {
        }

        private static void EnsureSql2008DbExists()
        {
            var connectionInfo = new Sql2008ConnectionInfo(TestConstants.ConnectionStringNameForSql2008);
            var database = new Sql2008DbFactory().CreateDatabase(connectionInfo);
            database.EnsureNewDatabase();
        }

        private static void EnsureSqlCe4DbExists()
        {
            //var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSqlCe4);
            //var database = new SqlCe4DbFactory().CreateDatabase(connectionInfo);
            //database.EnsureNewDatabase();
        }
    }
}