namespace SisoDb
{
    public class DbUoWExtensionPoint
    {
        public readonly ISisoDatabase Instance;

        public DbUoWExtensionPoint(ISisoDatabase instance)
        {
            Instance = instance;
        }
    }
}