using Machine.Specifications;

namespace SisoDb.Specifications.Sql2008
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