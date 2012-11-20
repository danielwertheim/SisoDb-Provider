using ApprovalTests.Reporters;
using NUnit.Framework;
using SisoDb.NCore;
using SisoDb.Testing;

namespace SisoDb.UnitTests
{
    [TestFixture]
    [UseReporter(typeof(DiffReporter))]
    public abstract class UnitTestBase
    {
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
            SysDateTime.NowFn = () => TestConstants.FixedDateTime;
            OnTestInitialize();
        }

        protected virtual void OnTestInitialize()
        {
        }

        [TearDown]
        public void TestFinalizer()
        {
            OnTestFinalize();
            SysDateTime.NowFn = () => TestConstants.FixedDateTime;   
        }

        protected virtual void OnTestFinalize()
        {
        }
    }
}