using NCore;
using NUnit.Framework;
using SisoDb.Resources;
using SisoDb.SqlCe4;

namespace SisoDb.UnitTests.Providers.Connections
{
    [TestFixture]
    public class SqlCe4ConnectionInfoTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenCnStringWithOnlyPlainPart_InstanceWithDefaultsIsCreated()
        {
            var cnString = new ConnectionString(@"data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;");

            var cnInfo = new SqlCe4ConnectionInfo(cnString);

            Assert.AreEqual(StorageProviders.SqlCe4, cnInfo.ProviderType);
            Assert.AreEqual(BackgroundIndexing.Off, cnInfo.BackgroundIndexing);
        }

        [Test]
        public void Ctor_WhenWrongProviderType_ThrowsSisoDbException()
        {
            var cnStringFake = Stub.This<IConnectionString>(
                o => o.Setup(s => s.Provider).Returns(StorageProviders.Sql2008.ToString));

            var ex = Assert.Throws<SisoDbException>(() => new SqlCe4ConnectionInfo(cnStringFake));

            Assert.AreEqual(ExceptionMessages.ConnectionInfo_UnsupportedProviderSpecified
                    .Inject(cnStringFake.Provider, StorageProviders.SqlCe4), ex.Message);
        }

        [Test]
        public void Ctor_WhenPassingDataSourceWithDataDirectory_ItRocks()
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

        [Test]
        public void Ctor_WhenCorrectConnectionString_PartsExtracted()
        {
            var cnString = new ConnectionString(@"sisodb:provider=SqlCe4;backgroundindexing=Off||plain:data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;Enlist=True");

            var cnInfo = new SqlCe4ConnectionInfo(cnString);

            Assert.AreEqual(StorageProviders.SqlCe4, cnInfo.ProviderType);
            Assert.AreEqual(BackgroundIndexing.Off, cnInfo.BackgroundIndexing);
            Assert.AreEqual("SisoDbTestsTemp", cnInfo.DbName);
            Assert.AreEqual(@"d:\#Temp\SisoDb", cnInfo.ServerPath);
            Assert.AreEqual(@"Data Source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;Enlist=False", cnInfo.ServerConnectionString.PlainString);
            Assert.AreEqual(@"Data Source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;Enlist=False", cnInfo.ClientConnectionString.PlainString);
        }

        [Test]
        public void Ctor_WhenBackgroundIndexingIsMissing_DefaultsToOff()
        {
            var cnString = new ConnectionString(@"sisodb:provider=SqlCe4||plain:data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;Enlist=True");

            var cnInfo = new SqlCe4ConnectionInfo(cnString);

            Assert.AreEqual(BackgroundIndexing.Off, cnInfo.BackgroundIndexing);
        }

        [Test]
        public void Ctor_WhenBackgroundIndexingIsOn_ThrowsSisoDbException()
        {
            var cnString = new ConnectionString(@"sisodb:provider=SqlCe4;backgroundindexing=On||plain:data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;Enlist=True");

            var ex = Assert.Throws<SisoDbException>(() => new SqlCe4ConnectionInfo(cnString));

            Assert.AreEqual(ExceptionMessages.ConnectionInfo_BackgroundIndexingNotSupported.Inject(StorageProviders.SqlCe4), ex.Message);
        }
    }
}