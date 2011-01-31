using System;
using System.Configuration;
using NUnit.Framework;
using SisoDb.TestUtils;
using TypeMock.ArrangeActAssert;

namespace SisoDb.Tests.UnitTests
{
    [TestFixture]
    public class SisoConnectionInfoTests : UnitTestBase
    {
        [Test, Isolated]
        public void ProviderType_WhenProviderNameIsSql2008_ValueIsReflectedInProperty()
        {
            const string connectionStringName = "Test";
            Isolate.Fake.StaticMethods(typeof(ConfigurationManager));
            Isolate.WhenCalled(() => ConfigurationManager.ConnectionStrings[connectionStringName])
                .WillReturn(new ConnectionStringSettings(connectionStringName, "sisodb:provider=Sql2008||plain:TestKey1=TestValue1"));

            var connectionInfo = new SisoConnectionInfo(connectionStringName);

            Assert.AreEqual(StorageProviders.Sql2008, connectionInfo.ProviderType);
        }

        [Test, Isolated]
        public void ConnectionString_WhenValueForPlainIsProvided_ValueIsReflectedInProperty()
        {
            const string connectionStringName = "Test";
            Isolate.Fake.StaticMethods(typeof(ConfigurationManager));
            Isolate.WhenCalled(() => ConfigurationManager.ConnectionStrings[connectionStringName])
                .WillReturn(new ConnectionStringSettings(connectionStringName, "sisodb:provider=Sql2008||plain:TestKey1=TestValue1"));

            var connectionInfo = new SisoConnectionInfo(connectionStringName);

            Assert.AreEqual("TestKey1=TestValue1", connectionInfo.ConnectionString.PlainString);
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
