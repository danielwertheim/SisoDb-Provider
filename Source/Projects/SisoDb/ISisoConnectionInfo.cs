namespace SisoDb
{
    public interface ISisoConnectionInfo
    {
        StorageProviders ProviderType { get; }
        string DbName { get; }
        string ClientConnectionString { get; }
        string ServerConnectionString { get; }
    }
}