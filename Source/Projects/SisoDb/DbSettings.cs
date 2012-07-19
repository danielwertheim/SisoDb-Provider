using System;
using SisoDb.EnsureThat;

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
        public bool AllowUpsertsOfSchemas { get; set; }

        protected DbSettings()
        {
            MaxInsertManyBatchSize = 500;
            MaxUpdateManyBatchSize = 500;
            SynchronizeSchemaChanges = true;
            AllowUpsertsOfSchemas = true;
        }

        public static IDbSettings CreateDefault()
        {
            return new DbSettings();
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