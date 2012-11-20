using System.Text;

namespace SisoDb.NCore.Cryptography
{
    public abstract class HashServiceBase : IHashService
    {
        protected readonly Encoding Encoding = Encoding.UTF8;

        public abstract string GenerateHash(string value);

        protected virtual string HashBytesToString(byte[] hash)
        {
            var result = new StringBuilder();

            foreach (var b in hash)
                result.Append(b.ToString("x2").ToLower());

            return result.ToString();
        }
    }
}