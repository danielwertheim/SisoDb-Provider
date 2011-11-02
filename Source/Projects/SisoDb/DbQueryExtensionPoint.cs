using System.Diagnostics;

namespace SisoDb
{
    [DebuggerStepThrough]
    public class DbQueryExtensionPoint
    {
        public readonly ISisoDatabase Instance;

        public DbQueryExtensionPoint(ISisoDatabase instance)
        {
            Instance = instance;
        }
    }
}