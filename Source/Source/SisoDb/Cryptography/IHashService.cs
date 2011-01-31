namespace SisoDb.Cryptography
{
    internal interface IHashService
    {
        int GetHashLength();

        string GenerateHash(string value);
    }
}