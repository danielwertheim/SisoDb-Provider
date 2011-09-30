namespace SisoDb.Cryptography
{
    public interface IHashService
    {
        int GetHashLength();

        string GenerateHash(string value);
    }
}