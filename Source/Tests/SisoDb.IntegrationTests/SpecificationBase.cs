using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.IntegrationTests
{
    public abstract class SpecificationBase
    {
        protected static ITestContext TestContext;

        Cleanup after = () => TestContext.Cleanup();
    }
}