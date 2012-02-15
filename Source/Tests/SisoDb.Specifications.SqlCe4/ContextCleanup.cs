using Machine.Specifications;

namespace SisoDb.Specifications.SqlCe4
{
    public class ContextCleanup : ICleanupAfterEveryContextInAssembly
    {
        public void AfterContextCleanup()
        {
            //SysDateTime.SetFixed(TestConstants.FixedDateTime);
        }
    }
}