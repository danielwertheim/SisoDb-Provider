using System;
using Machine.Specifications;

namespace SisoDb.Testing
{
    public abstract class SpecificationBase
    {
        protected static ITestContext TestContext;

        protected static Exception CaughtException;

        Cleanup after = () => 
        {
            TestContext.Cleanup();
            CaughtException = null;
        };
    }
}