namespace SisoDb
{
    public static class DbExtensionsPoints
    {
        public static DbQueryExtensionPoint FetchVia(this ISisoDatabase db)
        {
            return new DbQueryExtensionPoint(db);
        }

        public static DbUoWExtensionPoint UoW(this ISisoDatabase db)
        {
            return new DbUoWExtensionPoint(db);
        }
    }
}