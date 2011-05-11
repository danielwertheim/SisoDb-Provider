using System;
using System.Configuration;
using Moq;
using NUnit.Framework;
using SisoDb.TestUtils;

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
            var connectionInfo = new SisoConnectionInfo(connectionStringFake.Object);

            Assert.AreEqual(StorageProviders.Sql2008, connectionInfo.ProviderType);
        }

        [Test]
        public void Constructor_WhenNoConnectionStringOrNameIsPassed_ThrowsArgumentNullException()
        {
            CustomAssert.Throws<ArgumentNullException>(() => new SisoConnectionInfo(null as string));
        }

        [Test]
        public void Constructor_WhenNoConnectionStringIsPassed_ThrowsArgumentNullException()
        {
            CustomAssert.Throws<ArgumentNullException>(() => new SisoConnectionInfo(null as IConnectionString));
        }
    }
}
