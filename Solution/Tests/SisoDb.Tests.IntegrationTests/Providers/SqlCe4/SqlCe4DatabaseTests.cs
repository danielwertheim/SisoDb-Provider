using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Core.Io;
using SisoDb.Providers.SqlCe4;
using SisoDb.Resources;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlCe4
{
    [TestFixture]
    public class SqlCe4DatabaseTests : SqlCe4IntegrationTestBase
    {
        private readonly ISisoConnectionInfo _connectionInfoForTempDb = new SisoConnectionInfo(LocalConstants.ConnectionStringNameForSqlCe4Temp);

        [Test]
        public void Ctor_WhenConnectionInfoHasWrongProviderType_ThrowsSisoDbException()
        {
            var connectionInfoStub = Stub.This<ISisoConnectionInfo>(
                o => o.Setup(s => s.ProviderType).Returns(StorageProviders.Sql2008));

            var ex = Assert.Throws<SisoDbException>(() => new SqlCe4Database(connectionInfoStub));

            Assert.AreEqual(ExceptionMessages.SqlCe4Database_UnsupportedProviderSpecified.Inject(
                connectionInfoStub.ProviderType, StorageProviders.SqlCe4), ex.Message);
        }

        [Test]
        public void CreateIfNotExists_WhenItDoesNotExist_DatabaseGetsCreated()
        {
            var db = new SqlCe4Database(_connectionInfoForTempDb);

            IoHelper.DeleteIfFileExists(db.FilePath);

            Assert.IsFalse(IoHelper.FileExists(db.FilePath));

            db.CreateIfNotExists();

            Assert.IsTrue(IoHelper.FileExists(db.FilePath));
        }
    }
}