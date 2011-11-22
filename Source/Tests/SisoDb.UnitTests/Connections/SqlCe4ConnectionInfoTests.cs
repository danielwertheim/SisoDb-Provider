using NCore;
using NUnit.Framework;
using SisoDb.Resources;
using SisoDb.SqlCe4;

namespace SisoDb.UnitTests.Connections
{
    [TestFixture]
    public class SqlCe4ConnectionInfoTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenWrongProviderType_ThrowsSisoDbException()
        {
            var connectionInfoStub = Stub.This<IConnectionString>(
                o => o.Setup(s => s.Provider).Returns(StorageProviders.Sql2008.ToString));

            var ex = Assert.Throws<SisoDbException>(() => new SqlCe4ConnectionInfo(connectionInfoStub));

            Assert.AreEqual(ExceptionMessages.ConnectionInfo_UnsupportedProviderSpecified
                    .Inject(connectionInfoStub.Provider, StorageProviders.SqlCe4), ex.Message);
        }

        [Test]
        public void Ctor_WhenPassingDataSourceForWithDataDirectory_ItRocks()
        {
            var cnString = new ConnectionString(@"sisodb:provider=SqlCe4||plain:data source=|DataDirectory|SisoDbTestsTemp.sdf;");

            var cnInfo = new SqlCe4ConnectionInfo(cnString);

            Assert.AreEqual(StorageProviders.SqlCe4, cnInfo.ProviderType);
            Assert.AreEqual("SisoDbTestsTemp.sdf", cnInfo.DbName);
            Assert.AreEqual(@"SisoDbTestsTemp.sdf", cnInfo.FilePath);
        }

        [Test]
        public void Ctor_WhenPassingDataSourceWithFilePath_ProviderTypeIsExtracted()
        {
            var cnString = new ConnectionString(@"sisodb:provider=SqlCe4||plain:data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;");

            var cnInfo = new SqlCe4ConnectionInfo(cnString);

            Assert.AreEqual(StorageProviders.SqlCe4, cnInfo.ProviderType);
        }

        [Test]
        public void Ctor_WhenPassingDataSourceWithFilePath_FilePathIsExtracted()
        {
            var cnString = new ConnectionString(@"sisodb:provider=SqlCe4||plain:data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;");

            var cnInfo = new SqlCe4ConnectionInfo(cnString);

            Assert.AreEqual(@"d:\#Temp\SisoDb\SisoDbTestsTemp.sdf", cnInfo.FilePath);
        }

        [Test]
        public void Ctor_WhenPassingDataSourceWithFilePath_NameIsExtracted()
        {
            var cnString = new ConnectionString(@"sisodb:provider=SqlCe4||plain:data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;");

            var cnInfo = new SqlCe4ConnectionInfo(cnString);

            Assert.AreEqual("SisoDbTestsTemp", cnInfo.DbName);
        }
    }
}