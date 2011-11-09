namespace SisoDb
{
    public interface ISisoConnectionInfo
    {
        string DbName { get; }

        StorageProviders ProviderType { get; }

        IConnectionString ConnectionString { get; }

        IConnectionString ServerConnectionString { get; }
    }
}