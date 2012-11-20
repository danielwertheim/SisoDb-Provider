using System.Security.Cryptography;

namespace SisoDb.NCore.Cryptography
{
    public class Sha1HashService : HashServiceBase
    {
        private readonly SHA1 _hasher;

        public Sha1HashService()
        {
            _hasher = new SHA1CryptoServiceProvider();
        }

        public override string GenerateHash(string value)
        {
            return HashBytesToString(_hasher.ComputeHash(Encoding.GetBytes(value)));
        }
    }
}