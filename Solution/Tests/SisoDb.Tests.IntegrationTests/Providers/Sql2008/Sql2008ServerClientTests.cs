using NUnit.Framework;
using SisoDb.Providers.Sql2008;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008
{
    [TestFixture]
    public class Sql2008ServerClientTests : SqlIntegrationTestBase
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

            using (var serverClient = new Sql2008ServerClient((Sql2008ConnectionInfo)db.ConnectionInfo))
            {
                Assert.AreEqual("Data Source=.;Initial Catalog=;Integrated Security=True", serverClient.ConnectionString.PlainString);
            }
        }
    }
}