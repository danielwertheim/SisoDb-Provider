using NUnit.Framework;
using SisoDb.Providers.Sql2008Provider;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008Provider
{
    [TestFixture]
    public class SqlDbClientTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DbHelper.DropDatabase(LocalConstants.TempDbName);
        }

        [Test]
        public void ForServerConnection_WhenServerExists_ConnectionIsAgainstMaster()
        {
            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSql2008Temp);
            var db = new Sql2008Database(connectionInfo);

            using (var dbClient = new SqlDbClient(db.ServerConnectionInfo, false))
            {
                Assert.AreEqual("master", dbClient.DbName);
            }
        }

        [Test]
        public void ForDatabaseConnection_WhenDatabaseExists_ConnectionIsAgainstSpecifiedDb()
        {
            DbHelper.EnsureDbExists(LocalConstants.TempDbName);
            var connectionInfo = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSql2008Temp);
            var db = new Sql2008Database(connectionInfo);

            using (var dbClient = new SqlDbClient(db.ConnectionInfo, false))
            {
                Assert.AreEqual("SisoDb.IntegrationTests.Temp", dbClient.DbName);
            }
        }
    }
}