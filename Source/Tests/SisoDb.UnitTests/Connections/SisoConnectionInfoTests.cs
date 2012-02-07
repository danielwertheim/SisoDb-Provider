using System;
using Moq;
using NUnit.Framework;

namespace SisoDb.UnitTests.Connections
{
    [TestFixture]
    public class SisoConnectionInfoTests : UnitTestBase
    {
        [Test]
        public void Provider_WhenProviderNameIsSql2008_ValueIsReflectedInProperty()
        {
            var connectionStringFake = new Mock<IConnectionString>();
            connectionStringFake.Setup(f => f.Provider).Returns(StorageProviders.Sql2008.ToString());
            var connectionInfo = new SisoConnectionInfoImplementation(connectionStringFake.Object);

            Assert.AreEqual(StorageProviders.Sql2008, connectionInfo.ProviderType);
        }

        [Test]
        public void ParallelInserts_WhenValueIsSimple_ValueIsReflectedInProperty()
        {
            var connectionStringFake = new Mock<IConnectionString>();
            connectionStringFake.Setup(f => f.Provider).Returns(StorageProviders.Sql2008.ToString());
            connectionStringFake.Setup(f => f.ParallelInserts).Returns(ParallelInserts.Simple.ToString());
            var connectionInfo = new SisoConnectionInfoImplementation(connectionStringFake.Object);

            Assert.AreEqual(ParallelInserts.Simple, connectionInfo.ParallelInserts);
        }

        [Test]
        public void ParallelInserts_WhenNoValueIsSpecified_ValueIsNone()
        {
            var connectionStringFake = new Mock<IConnectionString>();
            connectionStringFake.Setup(f => f.Provider).Returns(StorageProviders.Sql2008.ToString());
            connectionStringFake.Setup(f => f.ParallelInserts).Returns(null as string);
            var connectionInfo = new SisoConnectionInfoImplementation(connectionStringFake.Object);

            Assert.AreEqual(ParallelInserts.None, connectionInfo.ParallelInserts);
        }

        [Test]
        public void Constructor_WhenNoConnectionStringOrNameIsPassed_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new SisoConnectionInfoImplementation(null as string));

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
