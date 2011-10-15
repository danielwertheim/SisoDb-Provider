namespace SisoDb.Specifications
{
    public interface ITestContext
    {
        ITestDbUtils DbHelper { get; }
        ITestDbUtils DbHelperForServer { get; }
        ISisoDatabase Database { get; }

        void Cleanup();
    }
}