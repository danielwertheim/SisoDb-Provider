using System;
using Machine.Specifications;
using SisoDb.NCore;

namespace SisoDb.Testing
{
    public abstract class SpecificationBase
    {
        protected static ITestContext TestContext;

        protected static Exception CaughtException;

        protected SpecificationBase()
        {
            SysDateTime.NowFn = () => TestConstants.FixedDateTime;   
        }

        Cleanup after = () => 
        {
            SysDateTime.NowFn = () => TestConstants.FixedDateTime;  

            if(TestContext != null)
                TestContext.Cleanup();

            CaughtException = null;
        };
    }
}