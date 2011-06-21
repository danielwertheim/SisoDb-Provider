using NUnit.Framework;
using SisoDb.Core;
using SisoDb.SqlCe4;
using SisoDb.SqlCe4.Resources;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.SqlCe4
{
    [TestFixture]
    public class SqlCe4ConnectionInfoTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenConnectionInfoHasWrongProviderType_ThrowsSisoDbException()
        {
            var connectionInfoStub = Stub.This<ISisoConnectionInfo>(
                o => o.Setup(s => s.ProviderType).Returns(StorageProviders.Sql2008));

            var ex = Assert.Throws<SisoDbException>(() => new SqlCe4ConnectionInfo(connectionInfoStub));

            Assert.AreEqual(
                SqlCe4Exceptions.SqlCe4Database_UnsupportedProviderSpecified
                    .Inject(connectionInfoStub.ProviderType, StorageProviders.SqlCe4), ex.Message);
        }

        [Test]
        public void Ctor_WhenPassingDataSourceWithFilePath_ProviderTypeIsExtracted()
        {
            var cnString = new ConnectionString(@"sisodb:provider=SqlCe4||plain:data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;");

            var cnInfoStub = Stub.This<ISisoConnectionInfo>(
                o => o.Setup(s => s.ProviderType).Returns(StorageProviders.SqlCe4),
                o => o.Setup(s => s.ConnectionString).Returns(cnString));

            var cnInfo = new SqlCe4ConnectionInfo(cnInfoStub);

            Assert.AreEqual(StorageProviders.SqlCe4, cnInfo.ProviderType);
        }

        [Test]
        public void Ctor_WhenPassingDataSourceWithFilePath_FilePathIsExtracted()
        {
            var cnString = new ConnectionString(@"sisodb:provider=SqlCe4||plain:data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;");

            var cnInfoStub = Stub.This<ISisoConnectionInfo>(
                o => o.Setup(s => s.ProviderType).Returns(StorageProviders.SqlCe4),
                o => o.Setup(s => s.ConnectionString).Returns(cnString));

            var cnInfo = new SqlCe4ConnectionInfo(cnInfoStub);

            Assert.AreEqual(@"d:\#Temp\SisoDb\SisoDbTestsTemp.sdf", cnInfo.FilePath);
        }

        [Test]
        public void Ctor_WhenPassingDataSourceWithFilePath_NameIsExtracted()
        {
            var cnString = new ConnectionString(@"sisodb:provider=SqlCe4||plain:data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;");

            var cnInfoStub = Stub.This<ISisoConnectionInfo>(
                o => o.Setup(s => s.ProviderType).Returns(StorageProviders.SqlCe4),
                o => o.Setup(s => s.ConnectionString).Returns(cnString));

            var cnInfo = new SqlCe4ConnectionInfo(cnInfoStub);

            Assert.AreEqual("SisoDbTestsTemp", cnInfo.Name);
        }
    }
}