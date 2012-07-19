namespace SisoDb.NCore
{
    public interface IStringConverter
    {
        string AsString<T>(T value);
    }
}