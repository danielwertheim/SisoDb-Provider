using Machine.Specifications;

namespace SisoDb.Specifications.Sql2012
{
    public class ContextCleanup : ICleanupAfterEveryContextInAssembly
    {
        public void AfterContextCleanup()
        {
            //SysDateTime.SetFixed(TestConstants.FixedDateTime);
        }
    }
}