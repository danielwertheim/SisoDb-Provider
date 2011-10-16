using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.Specifications
{
    public abstract class SpecificationBase
    {
        protected static ITestContext TestContext;

        Cleanup after = () => TestContext.Cleanup();
    }
}