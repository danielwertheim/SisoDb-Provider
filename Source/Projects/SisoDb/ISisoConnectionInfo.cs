namespace SisoDb
{
    public interface ISisoConnectionInfo
    {
        string DbName { get; }

        StorageProviders ProviderType { get; }

        ParallelInserts ParallelInserts { get; }

        IConnectionString ClientConnectionString { get; }

        IConnectionString ServerConnectionString { get; }
    }
}