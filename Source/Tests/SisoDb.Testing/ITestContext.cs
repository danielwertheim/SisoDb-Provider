namespace SisoDb.Testing
{
    public interface ITestContext
    {
        ISisoDatabase Database { get; }
        IDbProviderFactory ProviderFactory { get; }
        ITestDbUtils DbHelper { get; }
        ITestDbUtils DbHelperForServer { get; }

        void Cleanup();
    }
}