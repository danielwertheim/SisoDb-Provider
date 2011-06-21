using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Sql2008.DatabaseTests
{
    [TestFixture]
    public class InitializeExisting_WhenBlankDbExists_CreatesNewInitializedDb : Sql2008TembDbIntegrationTestBase
    {
        protected override string TempDbName
        {
            get { return "71E7A662DCF94C84A68EF9B306C830BD"; }
        }

        [Test]
        public void Test()
        {
            DbHelperForServer.EnsureDbExists(TempDbName);

            Database.InitializeExisting();

            AssertInitializedTempDbExists();
        }
    }
}