namespace SisoDb.Core
{
    public interface IStringConverter
    {
        string AsString<T>(T value);
    }
}