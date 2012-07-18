using System.Text;

namespace NCore.Cryptography
{
    public class Crc32HashService : IHashService
    {
        private readonly Crc32Algorithm _hasher = new Crc32Algorithm();
        private readonly Encoding _encoding = Encoding.UTF8;

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