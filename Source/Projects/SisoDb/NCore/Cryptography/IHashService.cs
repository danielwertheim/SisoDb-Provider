namespace SisoDb.NCore.Cryptography
{
    public interface IHashService
    {
        string GenerateHash(string value);
    }
}