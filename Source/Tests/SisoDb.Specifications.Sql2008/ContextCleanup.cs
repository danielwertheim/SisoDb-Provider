using Machine.Specifications;

namespace SisoDb.Specifications.Sql2008
{
    public class ContextCleanup : ICleanupAfterEveryContextInAssembly
    {
        public void AfterContextCleanup()
        {
            //SysDateTime.SetFixed(TestConstants.FixedDateTime);
            //FirestarterRuntime.Reinitialize();
        }
    }
}