using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests
{
    [TestFixture]
    public abstract class IntegrationTestBase<T> where T : ISisoDatabase
    {
        protected T Database { get; set; }

        protected IntegrationTestBase(ISisoDbFactory dbFactory, ISisoConnectionInfo connectionInfo)
        {
            Database = (T)dbFactory.CreateDatabase(connectionInfo);
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

        protected void DropStructureSet<TStructure>() where TStructure : class
        {
            Database.DropStructureSet<TStructure>();
        }
    }
}