namespace SisoDb
{
    public interface ISisoConnectionInfo
    {
        string DbName { get; }

        StorageProviders ProviderType { get; }

        ParallelInsertMode ParallelInsertMode { get; }

        IConnectionString ClientConnectionString { get; }

        IConnectionString ServerConnectionString { get; }
    }
}