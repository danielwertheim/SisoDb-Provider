using NUnit.Framework;
using SisoDb.Providers.Sql2008;
using SisoDb.Providers.Sql2008.Dac;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.Dac
{
    [TestFixture]
    public class Sql2008DbClientTests : Sql2008TembDbIntegrationTestBase
    {
        protected override string TempDbName
        {
            get { return "FA81E9F0F4904FE488BECF9B6005FAA2"; }
        }

        [Test]
        public void ForDatabaseConnection_WhenDatabaseExists_GainsCorrectConnectionString()
        {
            DbHelperForServer.EnsureDbExists(TempDbName);

            using (var dbClient = new Sql2008DbClient((Sql2008ConnectionInfo)Database.ConnectionInfo, false))
            {
                Assert.AreEqual("data source=.;initial catalog=FA81E9F0F4904FE488BECF9B6005FAA2;integrated security=SSPI;", dbClient.ConnectionString.PlainString);
            }
        }

        [Test]
        public void ForDatabaseConnection_WhenDatabaseExists_GainsCorrectProviderType()
        {
            DbHelperForServer.EnsureDbExists(TempDbName);

            using (var dbClient = new Sql2008DbClient((Sql2008ConnectionInfo)Database.ConnectionInfo, false))
            {
                Assert.AreEqual(StorageProviders.Sql2008, dbClient.ProviderType);
            }
        }

        [Test]
        public void ForDatabaseConnection_WhenDatabaseExists_ConnectionIsAgainstSpecifiedDb()
        {
            DbHelperForServer.EnsureDbExists(TempDbName);

            using (var dbClient = new Sql2008DbClient((Sql2008ConnectionInfo)Database.ConnectionInfo, false))
            {
                Assert.AreEqual("FA81E9F0F4904FE488BECF9B6005FAA2", dbClient.DbName);
            }
        }
    }
}