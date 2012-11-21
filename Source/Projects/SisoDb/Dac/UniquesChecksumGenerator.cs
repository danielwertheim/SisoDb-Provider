namespace SisoDb.Dac
{
    public static class UniquesChecksumGenerator
    {
        public static IDefaultUniquesChecksumGenerator Instance;

        static UniquesChecksumGenerator()
        {
            Instance = new DefaultUniquesChecksumGenerator();
        }
    }
}