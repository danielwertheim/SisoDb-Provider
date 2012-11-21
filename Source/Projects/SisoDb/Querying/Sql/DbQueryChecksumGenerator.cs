namespace SisoDb.Querying.Sql
{
    public static class DbQueryChecksumGenerator
    {
        public static IDbQueryChecksumGenerator Instance;

        static DbQueryChecksumGenerator()
        {
            Instance = new DefaultDbQueryChecksumGenerator();
        }
    }
}