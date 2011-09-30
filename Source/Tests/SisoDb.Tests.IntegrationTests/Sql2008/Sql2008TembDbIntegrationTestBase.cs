using System.Configuration;
using NUnit.Framework;
using SisoDb.Sql2008;

namespace SisoDb.Tests.IntegrationTests.Sql2008
{
    [TestFixture]
    public abstract class Sql2008TembDbIntegrationTestBase : Sql2008IntegrationTestBase
    {
        protected abstract string TempDbName { get; }

        protected Sql2008DbUtils DbHelperForServer;

        protected override void OnFixtureInitialize()
        {
            var cnString = string.Format(
                ConfigurationManager.ConnectionStrings[LocalConstants.ConnectionStringNameForSql2008Temp].ConnectionString,
                TempDbName);
            
            var cnInfo = new SqlConnectionInfo(cnString);

            Database = new SqlDatabase(cnInfo);
            DbHelper = new Sql2008DbUtils(cnInfo.ConnectionString.PlainString);
            DbHelperForServer = new Sql2008DbUtils(cnInfo.ServerConnectionString.PlainString);
        }

        protected override void OnTestInitialize()
        {
            DbHelperForServer.DropDatabase(TempDbName);
        }

        protected override void OnFixtureFinalize()
        {
            DbHelperForServer.DropDatabase(TempDbName);
        }

        protected void AssertInitializedTempDbExists()
        {
            var dbExists = Database.Exists();
            Assert.IsTrue(dbExists);

            var identitiesTableExists = DbHelper.TableExists("SisoDbIdentities");
            Assert.IsTrue(identitiesTableExists, "SisoDbIdentities table was not created");

            var guidIdsTypeExists = DbHelper.TypeExists("SisoGuidIds");
            var identityIdsTypeExists = DbHelper.TypeExists("SisoIdentityIds");
            Assert.IsTrue(guidIdsTypeExists);
            Assert.IsTrue(identityIdsTypeExists);
        }
    }
}