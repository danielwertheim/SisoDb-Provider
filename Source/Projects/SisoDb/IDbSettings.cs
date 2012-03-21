namespace SisoDb
{
    public interface IDbSettings 
    {
        int MaxInsertManyBatchSize { get; }
        int MaxUpdateManyBatchSize { get; }
    }
}