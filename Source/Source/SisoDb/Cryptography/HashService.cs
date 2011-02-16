using System.Text;

namespace SisoDb.Cryptography
{
    public class HashService : IHashService
    {
        private readonly Crc32Algorithm _hasher = new Crc32Algorithm();
        private readonly Encoding _encoding = SisoDbEnvironment.Encoding;

        public int GetHashLength()
        {
            return 8;
        }

        public string GenerateHash(string value)
        {
            var hash = _hasher.ComputeHash(_encoding.GetBytes(value));
            var result = new StringBuilder();

            foreach (var b in hash)
                result.Append(b.ToString("x2").ToLower());

            return result.ToString();
        }
    }
}