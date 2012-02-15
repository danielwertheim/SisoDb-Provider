namespace SisoDb
{
    public interface IConnectionString
    {
        string SisoDbString { get; }
        
        string PlainString { get; }

        string Provider { get; }

        string ParallelInserts { get; }

        IConnectionString ReplacePlain(string plainString);
    }
}