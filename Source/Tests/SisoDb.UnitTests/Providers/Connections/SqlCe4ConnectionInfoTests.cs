using NUnit.Framework;
using SisoDb.SqlCe4;

namespace SisoDb.UnitTests.Providers.Connections
{
    [TestFixture]
    public class SqlCe4ConnectionInfoTests : UnitTestBase
    {
        [Test]
        public void WhenPassingNormalCnString_ItRocks()
        {
            var cnInfo = new SqlCe4ConnectionInfo(@"data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;");

            Assert.AreEqual(StorageProviders.SqlCe4, cnInfo.ProviderType);
            Assert.AreEqual(@"Data Source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;Enlist=False", cnInfo.ServerConnectionString);
            Assert.AreEqual(@"Data Source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;Enlist=False", cnInfo.ClientConnectionString);
            Assert.AreEqual("SisoDbTestsTemp", cnInfo.DbName);
            Assert.AreEqual(@"d:\#Temp\SisoDb\SisoDbTestsTemp.sdf", cnInfo.FilePath);
        }

        [Test]
        public void WhenPassingEnlistTrue_EnlistBecomesFalse()
        {
            var cnInfo = new SqlCe4ConnectionInfo(@"data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;Enlist=True");

            Assert.AreEqual(@"Data Source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;Enlist=False", cnInfo.ServerConnectionString);
            Assert.AreEqual(@"Data Source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;Enlist=False", cnInfo.ClientConnectionString);
        }

        [Test]
        public void WhenPassingDataSourceWithDataDirectory_ItRocks()
        {
            var cnInfo = new SqlCe4ConnectionInfo(@"data source=|DataDirectory|SisoDbTestsTemp.sdf;");

            Assert.AreEqual(StorageProviders.SqlCe4, cnInfo.ProviderType);
            Assert.AreEqual(@"Data Source=|DataDirectory|SisoDbTestsTemp.sdf;Enlist=False", cnInfo.ServerConnectionString);
            Assert.AreEqual(@"Data Source=|DataDirectory|SisoDbTestsTemp.sdf;Enlist=False", cnInfo.ClientConnectionString);
            Assert.AreEqual("SisoDbTestsTemp", cnInfo.DbName);
            Assert.AreEqual(@"SisoDbTestsTemp.sdf", cnInfo.FilePath);
        }
    }
}