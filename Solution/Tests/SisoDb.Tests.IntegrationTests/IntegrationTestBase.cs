using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests
{
    [TestFixture]
    public abstract class IntegrationTestBase
    {
        protected ISisoDatabase Database { get; private set; }

        protected IntegrationTestBase(ISisoDbFactory dbFactory, string connectionStringName)
        {
            Database = dbFactory.CreateDatabase(new SisoConnectionInfo(connectionStringName));
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