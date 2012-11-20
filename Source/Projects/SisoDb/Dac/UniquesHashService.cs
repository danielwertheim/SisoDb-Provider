using SisoDb.NCore.Cryptography;

namespace SisoDb.Dac
{
    public static class UniquesHashService
    {
        public static IHashService Instance;

        static UniquesHashService()
        {
            Instance = new Crc32HashService();
        }
    }
}