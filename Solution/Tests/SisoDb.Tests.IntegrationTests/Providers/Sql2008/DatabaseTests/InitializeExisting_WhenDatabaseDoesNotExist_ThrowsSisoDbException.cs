using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.DatabaseTests
{
    [TestFixture]
    public class InitializeExisting_WhenDatabaseDoesNotExist_ThrowsSisoDbException : Sql2008TembDbIntegrationTestBase
    {
        protected override string TempDbName
        {
            get { return "8E3D12E9C0E14BD792769D01E70E2483"; }
        }

        [Test]
        public void Test()
        {
            DbHelperForServer.DropDatabase(TempDbName);

            Assert.Throws<SisoDbException>(Database.InitializeExisting);
        }
    }
}