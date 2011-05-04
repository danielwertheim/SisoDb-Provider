using NUnit.Framework;
using SisoDb.Providers.Sql2008;

namespace SisoDb.Tests.IntegrationTests
{
    [SetUpFixture]
    public class AssemblyInitalizer
    {
        [SetUp]
        public void Initialize()
        {
            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSql2008);
            var database = new Sql2008DbFactory().CreateDatabase(connectionInfo);
            database.EnsureNewDatabase();
        }
    }
}