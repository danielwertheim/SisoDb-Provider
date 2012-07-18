namespace NCore
{
    public interface IStringConverter
    {
        string AsString<T>(T value);
    }
}