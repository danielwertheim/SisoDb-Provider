using NUnit.Framework;
using SisoDb.Providers.SqlProvider;
using SisoDb.Resources;
using TypeMock.ArrangeActAssert;

namespace SisoDb.Tests.UnitTests.Providers.SqlProvider
{
    [TestFixture]
    public class SqlDatabaseTests : UnitTestBase
    {
        [Test, Isolated]
        public void CTor_ConnectionInfoWithNoDbName_ThrowsSisoDbException()
        {
            var cnInfo = Isolate.Fake.Instance<ISisoConnectionInfo>();
            Isolate.WhenCalled(() => cnInfo.ConnectionString.PlainString)
                .WillReturn("data source=localhost;integrated security=SSPI;");

            var ex = Assert.Throws<SisoDbException>(() => new SqlDatabase(cnInfo));

            Assert.AreEqual(ExceptionMessages.SqlDatabase_ConnectionInfo_MissingName, ex.Message);
        }

        [Test]
        public void CTor_ConnectionInfoWithDbName_ServerConnectionInfoGainsNoDbName()
        {
            var cnInfo = new SisoConnectionInfo(
                @"sisodb:provider=Sql2008||plain:Data Source=.\sqlexpress;Initial Catalog=DummyDb;Integrated Security=True");

            var db = new SqlDatabase(cnInfo);

            Assert.AreEqual(@"Data Source=.\sqlexpress;Initial Catalog=;Integrated Security=True", db.ServerConnectionInfo.ConnectionString.PlainString);
        }

        [Test]
        public void CTor_ConnectionInfoWithDbName_ExtractsCorrectDbName()
        {
            var cnInfo = new SisoConnectionInfo(
                @"sisodb:provider=Sql2008||plain:Data Source=.\sqlexpress;Initial Catalog=DummyDb;Integrated Security=True");

            var db = new SqlDatabase(cnInfo);

            Assert.AreEqual("DummyDb", db.Name);
        }
    }
}