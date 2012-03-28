using Machine.Specifications;

namespace SisoDb.Specifications.SqlCe4
{
    public class AssemblyContext : IAssemblyContext
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
            var ctx = TestContextFactory.Create();
            ctx.Database.EnsureNewDatabase();
        }
    }
}