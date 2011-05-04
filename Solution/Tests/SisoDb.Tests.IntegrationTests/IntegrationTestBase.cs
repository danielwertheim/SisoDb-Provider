using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests
{
    [TestFixture]
    public abstract class IntegrationTestBase
    {
        protected ISisoDatabase Database { get; private set; }

        protected IntegrationTestBase(string connectionStringName)
        {
            var connectionInfo = new SisoConnectionInfo(connectionStringName);
            Database = new SisoDbFactory().CreateDatabase(connectionInfo);
        }

        [TestFixtureSetUp]
        public void FixtureInitializer()
        {
            OnFixtureInitialize();
        }

        protected virtual void OnFixtureInitialize()
        {
        }

        [TestFixtureTearDown]
        public void FixtureFinalizer()
        {
            OnFixtureFinalize();
        }

        protected virtual void OnFixtureFinalize()
        {
        }

        [SetUp]
        public void TestInitializer()
        {
            OnTestInitialize();
        }

        protected virtual void OnTestInitialize()
        {
        }

        [TearDown]
        public void TestFinalizer()
        {
            OnTestFinalize();
            Database.StructureSchemas.Clear();
        }

        protected virtual void OnTestFinalize()
        {
        }

        protected void DropStructureSet<T>() where T : class
        {
            Database.DropStructureSet<T>();
        }
    }
}