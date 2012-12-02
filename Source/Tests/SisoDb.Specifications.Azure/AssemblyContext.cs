using Machine.Specifications;

namespace SisoDb.Specifications.Azure
{
	public class AssemblyContext : IAssemblyContext
	{
		private static bool _isInitialized;

		public void OnAssemblyStart()
		{
			if (_isInitialized)
				return;

			_isInitialized = true;

            //var ctx = TestContextFactory.Create();
            //ctx.Database.InitializeExisting();
		}

		public void OnAssemblyComplete()
		{
		}
	}
}