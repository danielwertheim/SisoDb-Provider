namespace SisoDb
{
    public class DbSettings : IDbSettings
    {
        public int MaxInsertManyBatchSize { get; private set; }
        public int MaxUpdateManyBatchSize { get; private set; }

        public DbSettings(int maxInsertManyBatchSize, int maxUpdateManyBatchSize)
        {
            MaxInsertManyBatchSize = maxInsertManyBatchSize;
            MaxUpdateManyBatchSize = maxUpdateManyBatchSize;
        }
    }
}