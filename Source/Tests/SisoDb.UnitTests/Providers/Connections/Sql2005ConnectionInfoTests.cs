using NCore;
using NUnit.Framework;
using SisoDb.Resources;
using SisoDb.Sql2005;

namespace SisoDb.UnitTests.Providers.Connections
{
    [TestFixture]
    public class Sql2005ConnectionInfoTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenCnStringWithOnlyPlainPart_InstanceWithDefaultsIsCreated()
        {
            var cnString = new ConnectionString(@"data source=.;initial catalog=foo;integrated security=SSPI;");

            var cnInfo = new Sql2005ConnectionInfo(cnString);

            Assert.AreEqual(StorageProviders.Sql2005, cnInfo.ProviderType);
            Assert.AreEqual(BackgroundIndexing.Off, cnInfo.BackgroundIndexing);
        }

        [Test]
        public void Ctor_WhenWrongProviderType_ThrowsSisoDbException()
        {
            var connectionInfoStub = Stub.This<IConnectionString>(
                o => o.Setup(s => s.Provider).Returns(StorageProviders.SqlCe4.ToString));

            var ex = Assert.Throws<SisoDbException>(() => new Sql2005ConnectionInfo(connectionInfoStub));

            Assert.AreEqual(ExceptionMessages.ConnectionInfo_UnsupportedProviderSpecified
                    .Inject(connectionInfoStub.Provider, StorageProviders.Sql2005), ex.Message);
        }

        [Test]
        public void Ctor_WhenMissingDbName_ThrowsSisoDbException()
        {
            var cnString = new ConnectionString(@"sisodb:provider=Sql2005||plain:data source=.;integrated security=SSPI;");

            var ex = Assert.Throws<SisoDbException>(() => new Sql2005ConnectionInfo(cnString));

            Assert.AreEqual(ExceptionMessages.ConnectionInfo_MissingName, ex.Message);
        }

        [Test]
        public void Ctor_WhenCorrectConnectionString_PartsExtracted()
        {
            var cnString = new ConnectionString(@"sisodb:provider=Sql2005;backgroundindexing=Off||plain:data source=.;initial catalog=SisoDbTests.Temp;integrated security=SSPI;");

            var cnInfo = new Sql2005ConnectionInfo(cnString);

            Assert.AreEqual(StorageProviders.Sql2005, cnInfo.ProviderType);
            Assert.AreEqual(BackgroundIndexing.Off, cnInfo.BackgroundIndexing);
            Assert.AreEqual("SisoDbTests.Temp", cnInfo.DbName);
            Assert.AreEqual(@"Data Source=.;Initial Catalog=;Integrated Security=True", cnInfo.ServerConnectionString.PlainString);
            Assert.AreEqual(@"data source=.;initial catalog=SisoDbTests.Temp;integrated security=SSPI;", cnInfo.ClientConnectionString.PlainString);
        }

        [Test]
        public void Ctor_WhenBackgroundIndexingIsMissing_DefaultsToOff()
        {
            var cnString = new ConnectionString(@"sisodb:provider=Sql2005||plain:data source=.;initial catalog=SisoDbTests.Temp;integrated security=SSPI;");

            var cnInfo = new Sql2005ConnectionInfo(cnString);

            Assert.AreEqual(BackgroundIndexing.Off, cnInfo.BackgroundIndexing);
        }

        [Test]
        public void Ctor_WhenBackgroundIndexingIsOn_ThrowsSisoDbException()
        {
            var cnString = new ConnectionString(@"sisodb:provider=Sql2005;backgroundindexing=On||plain:data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;Enlist=True");

            var ex = Assert.Throws<SisoDbException>(() => new Sql2005ConnectionInfo(cnString));

            Assert.AreEqual(ExceptionMessages.ConnectionInfo_BackgroundIndexingNotSupported.Inject(StorageProviders.Sql2005), ex.Message);
        }
    }
}