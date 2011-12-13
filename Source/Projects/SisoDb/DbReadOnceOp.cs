using System.Diagnostics;

namespace SisoDb
{
    [DebuggerStepThrough]
    public class DbReadOnceOp
    {
        public readonly ISisoDatabase Instance;

        public DbReadOnceOp(ISisoDatabase instance)
        {
            Instance = instance;
        }
    }
}