namespace SisoDb
{
    public interface IStringConverter
    {
        string AsString<T>(T value);
    }
}