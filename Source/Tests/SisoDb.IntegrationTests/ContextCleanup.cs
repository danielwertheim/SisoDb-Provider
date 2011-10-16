using Machine.Specifications;

namespace SisoDb.IntegrationTests
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