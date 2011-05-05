using Moq;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Providers.Sql2008;
using SisoDb.Providers.Sql2008.Resources;
using SisoDb.Resources;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Providers.Sql2008
{
    [TestFixture]
    public class Sql2008DatabaseTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenConnectionInfoHasWrongProviderType_ThrowsSisoDbException()
        {
            var connectionInfoStub = Stub.This<ISisoConnectionInfo>(
                o => o.Setup(s => s.ProviderType).Returns(StorageProviders.SqlCe4));

            var ex = Assert.Throws<SisoDbException>(() => new Sql2008Database(connectionInfoStub));

            Assert.AreEqual(Sql2008Exceptions.Sql2008Database_UnsupportedProviderSpecified
                .Inject(connectionInfoStub.ProviderType, StorageProviders.Sql2008), ex.Message);
        }

        [Test]
        public void CTor_ConnectionInfoWithNoDbName_ThrowsSisoDbException()
        {
            var cnInfo = new Mock<ISisoConnectionInfo>();
            cnInfo.Setup(x=>x.ConnectionString.PlainString)
                .Returns("data source=localhost;integrated security=SSPI;");

            var ex = Assert.Throws<SisoDbException>(() => new Sql2008Database(cnInfo.Object));

            Assert.AreEqual(ExceptionMessages.SqlDatabase_ConnectionInfo_MissingName, ex.Message);
        }

        [Test]
        public void CTor_SisoConnectionInfoIsPassed_ConnectionInfoOnDbIsSql2008ConnectionInfo()
        {
            var cnInfo = new SisoConnectionInfo(
                @"sisodb:provider=Sql2008||plain:Data Source=.\sqlexpress;Initial Catalog=DummyDb;Integrated Security=True");

            var db = new Sql2008Database(cnInfo);

            Assert.IsInstanceOf(typeof(Sql2008ConnectionInfo), db.ConnectionInfo);
        }
    }
}