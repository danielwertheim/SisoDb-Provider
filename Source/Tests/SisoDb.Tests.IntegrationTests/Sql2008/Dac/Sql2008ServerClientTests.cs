using NUnit.Framework;
using SisoDb.Sql2008;
using SisoDb.Sql2008.Dac;

namespace SisoDb.Tests.IntegrationTests.Sql2008.Dac
{
    [TestFixture]
    public class SqlServerClientTests : Sql2008TembDbIntegrationTestBase
    {
        protected override string TempDbName
        {
            get { return "DAD8CF7872044B4795C4034F5DF37C6E"; }
        }

        [Test]
        public void ForServerConnection_WhenServerExists_ConnectionIsAgainstMaster()
        {
            using (var serverClient = new SqlServerClient((SqlConnectionInfo)Database.ConnectionInfo))
            {
                Assert.AreEqual("Data Source=.\\sqlexpress;Initial Catalog=;Integrated Security=True", serverClient.ConnectionString.PlainString);
            }
        }
    }
}