using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Sql2008;
using SisoDb.Sql2008.Resources;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Sql2008
{
    [TestFixture]
    public class SqlConnectionInfoTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenConnectionInfoHasWrongProviderType_ThrowsSisoDbException()
        {
            var connectionInfoStub = Stub.This<ISisoConnectionInfo>(
                o => o.Setup(s => s.ProviderType).Returns(StorageProviders.SqlCe4));

            var ex = Assert.Throws<SisoDbException>(() => new Sql2008ConnectionInfo(connectionInfoStub));

            Assert.AreEqual(
                Sql2008Exceptions.SqlDatabase_UnsupportedProviderSpecified
                    .Inject(connectionInfoStub.ProviderType, StorageProviders.Sql2008), ex.Message);
        }

        [Test]
        public void Ctor_WhenPassingDataSourceWithFilePath_ProviderTypeIsExtracted()
        {
            var cnString = new ConnectionString(@"sisodb:provider=Sql2008||plain:data source=.;initial catalog=SisoDbTests.Temp;integrated security=SSPI;");

            var cnInfoStub = Stub.This<ISisoConnectionInfo>(
                o => o.Setup(s => s.ProviderType).Returns(StorageProviders.Sql2008),
                o => o.Setup(s => s.ConnectionString).Returns(cnString));

            var cnInfo = new Sql2008ConnectionInfo(cnInfoStub);

            Assert.AreEqual(StorageProviders.Sql2008, cnInfo.ProviderType);
        }

        [Test]
        public void Ctor_WhenPassingDataSourceWithFilePath_NameIsExtracted()
        {
            var cnString = new ConnectionString(@"sisodb:provider=Sql2008||plain:data source=.;initial catalog=SisoDbTests.Temp;integrated security=SSPI;");

            var cnInfoStub = Stub.This<ISisoConnectionInfo>(
                o => o.Setup(s => s.ProviderType).Returns(StorageProviders.Sql2008),
                o => o.Setup(s => s.ConnectionString).Returns(cnString));

            var cnInfo = new Sql2008ConnectionInfo(cnInfoStub);

            Assert.AreEqual("SisoDbTests.Temp", cnInfo.Name);
        }

        [Test]
        public void Ctor_WhenCnStringWithDb_ServerConnectionStringIsAgainstNoDb()
        {
            var cnString = new ConnectionString(@"sisodb:provider=Sql2008||plain:data source=.;initial catalog=SisoDbTests.Temp;integrated security=SSPI;");

            var cnInfoStub = Stub.This<ISisoConnectionInfo>(
                o => o.Setup(s => s.ProviderType).Returns(StorageProviders.Sql2008),
                o => o.Setup(s => s.ConnectionString).Returns(cnString));

            var cnInfo = new Sql2008ConnectionInfo(cnInfoStub);

            Assert.AreEqual(@"Data Source=.;Initial Catalog=;Integrated Security=True", cnInfo.ServerConnectionString.PlainString);
        }
    }
}