namespace SisoDb.Serialization
{
    public interface IJsonSerializer
    {
        string ToJsonOrEmptyString<T>(T item) where T : class;

        T ToItemOrNull<T>(string json) where T : class;
    }
}