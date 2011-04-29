using Moq;
using NUnit.Framework;
using SisoDb.Providers.SqlProvider;
using SisoDb.Resources;

namespace SisoDb.Tests.UnitTests.Providers.SqlProvider
{
    [TestFixture]
    public class SqlDatabaseTests : UnitTestBase
    {
        [Test]
        public void CTor_ConnectionInfoWithNoDbName_ThrowsSisoDbException()
        {
            var cnInfo = new Mock<ISisoConnectionInfo>();
            cnInfo.Setup(x=>x.ConnectionString.PlainString)
                .Returns("data source=localhost;integrated security=SSPI;");

            var ex = Assert.Throws<SisoDbException>(() => new SqlDatabase(cnInfo.Object));

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