using Machine.Specifications;
using SisoDb.SqlCe4;
using SisoDb.Testing;

namespace SisoDb.Specifications.SqlCe4
{
    public class AssemblyInitializer : IAssemblyContext
    {
        private static bool _isInitialized;

        public void OnAssemblyStart()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            EnsureDbExists();
        }

        public void OnAssemblyComplete()
        {
        }

        private static void EnsureDbExists()
        {
            var connectionInfo = new SqlCe4ConnectionInfo(TestConstants.ConnectionStringNameForSqlCe4);
            var database = new SqlCe4DbFactory().CreateDatabase(connectionInfo);
            database.EnsureNewDatabase();
        }
    }
}