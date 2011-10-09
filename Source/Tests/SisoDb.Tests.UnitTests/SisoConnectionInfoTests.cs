using System;
using Moq;
using NUnit.Framework;

namespace SisoDb.Tests.UnitTests
{
    [TestFixture]
    public class SisoConnectionInfoTests : UnitTestBase
    {
        [Test]
        public void ProviderType_WhenProviderNameIsSql2008_ValueIsReflectedInProperty()
        {
            var connectionStringFake = new Mock<IConnectionString>();
            connectionStringFake.Setup(f => f.Provider).Returns("Sql2008");
            var connectionInfo = new SisoConnectionInfoImplementation("");

            Assert.AreEqual(StorageProviders.Sql2008, connectionInfo.ProviderType);
        }

        [Test]
        public void Constructor_WhenNoConnectionStringOrNameIsPassed_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new SisoConnectionInfoImplementation(null as string));

            Assert.AreEqual("connectionStringOrName", ex.ParamName);
        }

        [Test]
        public void Constructor_WhenNoConnectionStringIsPassed_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new SisoConnectionInfoImplementation(null as IConnectionString));

            Assert.AreEqual("connectionString", ex.ParamName);
        }

        private class SisoConnectionInfoImplementation : SisoConnectionInfo
        {
            public SisoConnectionInfoImplementation(IConnectionString connectionString) : base(connectionString)
            {}

            public SisoConnectionInfoImplementation(string connectionStringOrName) : base(connectionStringOrName)
            {}

            public override string DbName
            {
                get { throw new NotImplementedException(); }
            }

            public override IConnectionString ServerConnectionString
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
