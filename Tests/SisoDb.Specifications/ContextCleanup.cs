using Machine.Specifications;

namespace SisoDb.Specifications
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