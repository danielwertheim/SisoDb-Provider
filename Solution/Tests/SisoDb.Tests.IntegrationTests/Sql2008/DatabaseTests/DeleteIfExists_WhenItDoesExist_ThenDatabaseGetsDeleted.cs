using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Sql2008.DatabaseTests
{
    [TestFixture]
    public class DeleteIfExists_WhenItDoesExist_ThenDatabaseGetsDeleted : Sql2008TembDbIntegrationTestBase
    {
        protected override string TempDbName
        {
            get { return "31A26B2A01A345BC90B8A0EB70A013C8"; }
        }

        [Test]
        public void Test()
        {
            DbHelperForServer.EnsureDbExists(TempDbName);

            Database.DeleteIfExists();

            Assert.IsFalse(Database.Exists());
        }
    }
}