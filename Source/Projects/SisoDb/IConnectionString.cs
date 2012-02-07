namespace SisoDb
{
    public interface IConnectionString
    {
        string SisoDbString { get; }
        
        string PlainString { get; }

        string Provider { get; }

        string ParallelInsertMode { get; }

        IConnectionString ReplacePlain(string plainString);
    }
}