using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests
{
    [SetUpFixture]
    public class AssemblyInitalizer
    {
        [SetUp]
        public void Initialize()
        {
            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSql2008);
            var database = new SisoDbFactory().CreateDatabase(connectionInfo);
            database.EnsureNewDatabase();
        }
    }
}