namespace SisoDb
{
    public interface ISisoConnectionInfo
    {
        string DbName { get; }

        StorageProviders ProviderType { get; }

        BackgroundIndexing BackgroundIndexing { get; }

        IConnectionString ClientConnectionString { get; }

        IConnectionString ServerConnectionString { get; }
    }
}