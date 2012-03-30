using System;
using EnsureThat;

namespace SisoDb
{
    public class DbSettings : IDbSettings
    {
        private int _maxInsertManyBatchSize;
        private int _maxUpdateManyBatchSize;

        public int MaxInsertManyBatchSize
        {
            get { return _maxInsertManyBatchSize; }
            set
            {
                Ensure.That(value, "MaxInsertManyBatchSize").IsGt(0);
                _maxInsertManyBatchSize = value;
            }
        }
        
        public int MaxUpdateManyBatchSize
        {
            get { return _maxUpdateManyBatchSize; }
            set
            {
                Ensure.That(value, "MaxUpdateManyBatchSize").IsGt(0);
                _maxUpdateManyBatchSize = value;
            }
        }

        public bool SynchronizeSchemaChanges { get; set; }

        protected DbSettings(int maxInsertManyBatchSize, int maxUpdateManyBatchSize, bool synchronizeSchemaChanges)
        {
            MaxInsertManyBatchSize = maxInsertManyBatchSize;
            MaxUpdateManyBatchSize = maxUpdateManyBatchSize;
            SynchronizeSchemaChanges = synchronizeSchemaChanges;
        }

        public static IDbSettings CreateDefault()
        {
            return new DbSettings(500, 500, true);
        }

        public static IDbSettings Create(Action<IDbSettings> initializer)
        {
            Ensure.That(initializer, "initializer").IsNotNull();

            var settings = CreateDefault();

            initializer.Invoke(settings);

            return settings;
        }
    }
}