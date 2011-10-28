namespace SisoDb
{
    public class DbQueryExtensionPoint
    {
        public readonly ISisoDatabase Instance;

        public DbQueryExtensionPoint(ISisoDatabase instance)
        {
            Instance = instance;
        }
    }
}