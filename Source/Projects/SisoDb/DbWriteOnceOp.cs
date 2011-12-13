using System.Diagnostics;

namespace SisoDb
{
    [DebuggerStepThrough]
    public class DbWriteOnceOp
    {
        public readonly ISisoDatabase Instance;

        public DbWriteOnceOp(ISisoDatabase instance)
        {
            Instance = instance;
        }
    }
}