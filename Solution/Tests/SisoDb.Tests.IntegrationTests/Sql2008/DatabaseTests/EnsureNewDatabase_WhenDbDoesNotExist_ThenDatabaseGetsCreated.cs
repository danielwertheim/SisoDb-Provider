using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Sql2008.DatabaseTests
{
    [TestFixture]
    public class EnsureNewDatabase_WhenDbDoesNotExist_ThenDatabaseGetsCreated : Sql2008TembDbIntegrationTestBase
    {
        protected override string TempDbName
        {
            get { return "012B774DD5C846E3B7295F8ACFEDD817"; }
        }

        [Test]
        public void Test()
        {
            DbHelperForServer.DropDatabase(TempDbName);

            Database.EnsureNewDatabase();

            AssertInitializedTempDbExists();
        }
    }
}