namespace SisoDb.NCore.Cryptography
{
    public class Crc32HashService : HashServiceBase
    {
        private readonly Crc32Algorithm _hasher = new Crc32Algorithm();

        public override string GenerateHash(string value)
        {
            return HashBytesToString(_hasher.ComputeHash(Encoding.GetBytes(value)));
        }
    }
}