using System;
using Moq;
using NCore;
using NUnit.Framework;
using SisoDb.Resources;

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
        public void BackgroundIndexing_WhenValueIsOn_ThrowsSisoDbException()
        {
            var connectionStringFake = new Mock<IConnectionString>();
            connectionStringFake.Setup(f => f.Provider).Returns(StorageProviders.Sql2008.ToString());
            connectionStringFake.Setup(f => f.BackgroundIndexing).Returns(BackgroundIndexing.On.ToString());

            var ex = Assert.Throws<SisoDbException>(() => new SisoConnectionInfoImplementation(connectionStringFake.Object));

            Assert.AreEqual(ExceptionMessages.ConnectionInfo_BackgroundIndexingNotSupported.Inject(StorageProviders.Sql2008), ex.Message);
        }

        [Test]
        public void BackgroundIndexing_WhenValueIsOff_ValueBecomesOff()
        {
            var connectionStringFake = new Mock<IConnectionString>();
            connectionStringFake.Setup(f => f.Provider).Returns(StorageProviders.Sql2008.ToString());
            connectionStringFake.Setup(f => f.BackgroundIndexing).Returns(BackgroundIndexing.Off.ToString());
            var connectionInfo = new SisoConnectionInfoImplementation(connectionStringFake.Object);

            Assert.AreEqual(BackgroundIndexing.Off, connectionInfo.BackgroundIndexing);
        }

        [Test]
        public void BackgroundIndexing_WhenNoValueIsSpecified_ValueBecomesOff()
        {
            var connectionStringFake = new Mock<IConnectionString>();
            connectionStringFake.Setup(f => f.Provider).Returns(StorageProviders.Sql2008.ToString());
            connectionStringFake.Setup(f => f.BackgroundIndexing).Returns(null as string);
            var connectionInfo = new SisoConnectionInfoImplementation(connectionStringFake.Object);

            Assert.AreEqual(BackgroundIndexing.Off, connectionInfo.BackgroundIndexing);
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

            public override StorageProviders ProviderType
            {
                get { return StorageProviders.Sql2008; }
            }

            public override string DbName
            {
                get { return "Foo"; }
            }
        }
    }
}
