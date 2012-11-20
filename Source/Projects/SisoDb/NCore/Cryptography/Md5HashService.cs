using System.Security.Cryptography;

namespace SisoDb.NCore.Cryptography
{
    public class Md5HashService : HashServiceBase
    {
        private readonly MD5 _hasher;

        public Md5HashService()
        {
            _hasher = new MD5CryptoServiceProvider();
        }

        public override string GenerateHash(string value)
        {
            return HashBytesToString(_hasher.ComputeHash(Encoding.GetBytes(value)));
        }
    }
}