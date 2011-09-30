namespace SisoDb
{
    public interface ISisoConnectionInfo
    {
        StorageProviders ProviderType { get; }

        IConnectionString ConnectionString { get; }
    }
}