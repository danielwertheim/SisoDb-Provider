using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.DatabaseTests
{
    [TestFixture]
    public class CreateIfNotExists_WhenDbDoesNotExist_ThenDatabaseGetsCreated : Sql2008TembDbIntegrationTestBase
    {
        protected override string TempDbName
        {
            get { return "D75214BAE3144FBD8DF65613AD8CDEA7"; }
        }

        [Test]
        public void Test()
        {
            DbHelperForServer.DropDatabase(TempDbName);

            Database.CreateIfNotExists();

            AssertInitializedTempDbExists();
        }
    }
}