using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Providers.SqlCe4;
using SisoDb.Providers.SqlCe4.Resources;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Providers.SqlCe4
{
    [TestFixture]
    public class SqlCe4DatabaseTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenConnectionInfoHasWrongProviderType_ThrowsSisoDbException()
        {
            var connectionInfoStub = Stub.This<ISisoConnectionInfo>(
                o => o.Setup(s => s.ProviderType).Returns(StorageProviders.Sql2008));

            var ex = Assert.Throws<SisoDbException>(() => new SqlCe4Database(connectionInfoStub));

            Assert.AreEqual(SqlCe4Exceptions.SqlCe4Database_UnsupportedProviderSpecified.Inject(connectionInfoStub.ProviderType, StorageProviders.SqlCe4), ex.Message);
        }

        [Test]
        public void CTor_SisoConnectionInfoIsPassed_ConnectionInfoOnDbIsSql2008ConnectionInfo()
        {
            var cnInfo = new SisoConnectionInfo(
                @"sisodb:provider=SqlCe4||plain:data source=d:\#Temp\SisoDb\SisoDbTestsTemp.sdf;");

            var db = new SqlCe4Database(cnInfo);

            Assert.IsInstanceOf(typeof(SqlCe4ConnectionInfo), db.ConnectionInfo);
        }
    }
}