using NUnit.Framework;
using SisoDb.Resources;
using SisoDb.Sql2008;

namespace SisoDb.UnitTests.Providers.Connections
{
    [TestFixture]
    public class Sql2008ConnectionInfoTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenPassingNormalCnString_ItRocks()
        {
            var cnInfo = new Sql2008ConnectionInfo(@"data source=.;initial catalog=SisoDbTests.Temp;integrated security=SSPI;");

            Assert.AreEqual(StorageProviders.Sql2008, cnInfo.ProviderType);
            Assert.AreEqual(@"Data Source=.;Initial Catalog=;Integrated Security=True", cnInfo.ServerConnectionString);
            Assert.AreEqual(@"Data Source=.;Initial Catalog=SisoDbTests.Temp;Integrated Security=True", cnInfo.ClientConnectionString);
            Assert.AreEqual("SisoDbTests.Temp", cnInfo.DbName);
        }

        [Test]
        public void Ctor_WhenMissingDbName_ThrowsSisoDbException()
        {
            var ex = Assert.Throws<SisoDbException>(() => new Sql2008ConnectionInfo(@"data source=.;integrated security=SSPI;"));

            Assert.AreEqual(ExceptionMessages.ConnectionInfo_MissingName, ex.Message);
        }
    }
}