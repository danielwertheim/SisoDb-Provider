using Machine.Specifications;

namespace SisoDb.Specifications.SqlProfiler
{
    public class ContextCleanup : ICleanupAfterEveryContextInAssembly
    {
        public void AfterContextCleanup()
        {
            //SysDateTime.SetFixed(TestConstants.FixedDateTime);
        }
    }
}