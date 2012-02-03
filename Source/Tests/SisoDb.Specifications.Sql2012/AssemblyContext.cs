using Machine.Specifications;
using SisoDb.Sql2012;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2012
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
			var connectionInfo = new Sql2012ConnectionInfo(TestConstants.ConnectionStringNameForSql2012);
			var database = new Sql2012DbFactory().CreateDatabase(connectionInfo);
			database.EnsureNewDatabase();
		}
	}
}