using Machine.Specifications;
using SisoDb.Sql2008;
using SisoDb.Testing;

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
			var connectionInfo = new Sql2008ConnectionInfo(TestConstants.ConnectionStringNameForSql2008);
			var database = new Sql2008DbFactory().CreateDatabase(connectionInfo);
			database.EnsureNewDatabase();
		}
	}
}