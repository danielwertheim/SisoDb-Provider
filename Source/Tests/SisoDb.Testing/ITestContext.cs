namespace SisoDb.Testing
{
    public interface ITestContext
    {
        ISisoDatabase Database { get; }
        ITestDbUtils DbHelper { get; }

        void Cleanup();
    }
}