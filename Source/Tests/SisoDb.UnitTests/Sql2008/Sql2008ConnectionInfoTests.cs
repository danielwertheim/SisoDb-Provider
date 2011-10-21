using NCore;
using NUnit.Framework;
using SisoDb.Sql2008;
using SisoDb.Sql2008.Resources;

namespace SisoDb.UnitTests.Sql2008
{
    [TestFixture]
    public class Sql2008ConnectionInfoTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenWrongProviderType_ThrowsSisoDbException()
        {
            var connectionInfoStub = Stub.This<IConnectionString>(
                o => o.Setup(s => s.Provider).Returns(StorageProviders.SqlCe4.ToString));

            var ex = Assert.Throws<SisoDbException>(() => new Sql2008ConnectionInfo(connectionInfoStub));

            Assert.AreEqual(
                Sql2008Exceptions.ConnectionInfo_UnsupportedProviderSpecified
                    .Inject(connectionInfoStub.Provider, StorageProviders.Sql2008), ex.Message);
        }

        [Test]
        public void Ctor_WhenMissingDbName_ThrowsSisoDbException()
        {
            var cnString = new ConnectionString(@"sisodb:provider=Sql2008||plain:data source=.;integrated security=SSPI;");

            var ex = Assert.Throws<SisoDbException>(() => new Sql2008ConnectionInfo(cnString));

            Assert.AreEqual(Sql2008Exceptions.ConnectionInfo_MissingName, ex.Message);
        }

        [Test]
        public void Ctor_WhenCorrectConnectionString_PartsExtracted()
        {
            var cnString = new ConnectionString(@"sisodb:provider=Sql2008||plain:data source=.;initial catalog=SisoDbTests.Temp;integrated security=SSPI;");

            var cnInfo = new Sql2008ConnectionInfo(cnString);

            Assert.AreEqual(StorageProviders.Sql2008, cnInfo.ProviderType);
            Assert.AreEqual("SisoDbTests.Temp", cnInfo.DbName);
            Assert.AreEqual(@"Data Source=.;Initial Catalog=;Integrated Security=True", cnInfo.ServerConnectionString.PlainString);
        }
    }
}