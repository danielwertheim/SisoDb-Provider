using System.Diagnostics;

namespace SisoDb
{
    [DebuggerStepThrough]
    public class DbUoWExtensionPoint
    {
        public readonly ISisoDatabase Instance;

        public DbUoWExtensionPoint(ISisoDatabase instance)
        {
            Instance = instance;
        }
    }
}