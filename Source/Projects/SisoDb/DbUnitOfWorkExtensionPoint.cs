using System.Diagnostics;

namespace SisoDb
{
    [DebuggerStepThrough]
    public class DbUnitOfWorkExtensionPoint
    {
        public readonly ISisoDatabase Instance;

        public DbUnitOfWorkExtensionPoint(ISisoDatabase instance)
        {
            Instance = instance;
        }
    }
}