using NUnit.Framework;
using SisoDb.Providers.Sql2008;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008
{
    [TestFixture]
    public class Sql2008DbClientTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DbHelper.DropDatabase(LocalConstants.TempDbName);
        }

        [Test]
        public void ForDatabaseConnection_WhenDatabaseExists_GainsCorrectConnectionString()
        {
            DbHelper.EnsureDbExists(LocalConstants.TempDbName);

            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSql2008Temp);
            var db = new Sql2008Database(connectionInfo);

            using (var dbClient = new Sql2008DbClient((Sql2008ConnectionInfo)db.ConnectionInfo, false))
            {
                Assert.AreEqual("data source=.;initial catalog=SisoDbTests.Temp;integrated security=SSPI;", dbClient.ConnectionString.PlainString);
            }
        }

        [Test]
        public void ForDatabaseConnection_WhenDatabaseExists_GainsCorrectProviderType()
        {
            DbHelper.EnsureDbExists(LocalConstants.TempDbName);

            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSql2008Temp);
            var db = new Sql2008Database(connectionInfo);

            using (var dbClient = new Sql2008DbClient((Sql2008ConnectionInfo)db.ConnectionInfo, false))
            {
                Assert.AreEqual(StorageProviders.Sql2008, dbClient.ProviderType);
            }
        }

        [Test]
        public void ForDatabaseConnection_WhenDatabaseExists_ConnectionIsAgainstSpecifiedDb()
        {
            DbHelper.EnsureDbExists(LocalConstants.TempDbName);

            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSql2008Temp);
            var db = new Sql2008Database(connectionInfo);

            using (var dbClient = new Sql2008DbClient((Sql2008ConnectionInfo)db.ConnectionInfo, false))
            {
                Assert.AreEqual("SisoDbTests.Temp", dbClient.DbName);
            }
        }
    }
}