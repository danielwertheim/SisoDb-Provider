using System;
using EnsureThat;
using NCore.Collections;
using PineCone.Serializers;
using PineCone.Structures.Schemas;
using SisoDb.Caching;

namespace SisoDb.Maintenance
{
    public class SisoDatabaseMaintenance : ISisoDatabaseMaintenance
    {
        private readonly ISisoDatabase _db;

        public SisoDatabaseMaintenance(ISisoDatabase db)
        {
            _db = db;
        }

        public virtual void Reset()
        {
            lock (_db.LockObject)
            {
                OnClearCache();

                using (var dbClient = _db.ProviderFactory.GetTransactionalDbClient(_db.ConnectionInfo))
                {
                    dbClient.Reset();
                }
            }
        }

        public virtual void RenameStructure(string @from, string to)
        {
            Ensure.That(@from).IsNotNullOrWhiteSpace();
            Ensure.That(to).IsNotNullOrWhiteSpace();

            lock (_db.LockObject)
            {
                _db.SchemaManager.RemoveFromCache(@from);

                using (var dbClient = _db.ProviderFactory.GetTransactionalDbClient(_db.ConnectionInfo))
                {
                    dbClient.RenameStructureSet(@from, to);
                }
            }
        }

        public virtual void RegenerateQueryIndexesFor<T>() where T : class
        {
            lock (_db.LockObject)
            {
                using (var dbClient = _db.ProviderFactory.GetTransactionalDbClient(_db.ConnectionInfo))
                {
                    var structureSchema = _db.StructureSchemas.GetSchema<T>();
                    _db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);

                    dbClient.ClearQueryIndexes(structureSchema);

                    var structureBuilder = _db.StructureBuilders.ForUpdates(structureSchema);
                    structureBuilder.StructureSerializer = new EmptyStructureSerializer(); //We don't need Json to regenerate Query indexes.

                    var structureInserter = _db.ProviderFactory.GetStructureInserter(dbClient);

                    foreach (var structuresBatch in _db.Serializer.DeserializeMany<T>(
                        dbClient.GetJsonOrderedByStructureId(structureSchema)).Batch(_db.Settings.MaxUpdateManyBatchSize))
                    {
                        structureInserter.InsertIndexesOnly(structureSchema, structureBuilder.CreateStructures(structuresBatch, structureSchema));
                    }
                }
            }
        }

        public virtual void Migrate<TFrom, TTo>(Func<TFrom, TTo, MigrationStatuses> modifier) 
            where TFrom : class 
            where TTo : class
        {
            Ensure.That(modifier, "modifier").IsNotNull();

            Migrate(new Migration<TFrom, TTo>(modifier));
        }

        public virtual void Migrate<TFrom, TTo>(Migration<TFrom, TTo> migration)
            where TFrom : class
            where TTo : class
        {
            Ensure.That(migration, "migration").IsNotNull();

            lock (_db.LockObject)
            {
                var migrator = new DbStructureSetMigrator(_db);
                migrator.Migrate(migration);
            }
        }

        public void Migrate<TFrom, TFromTemplate, TTo>(Migration<TFrom, TFromTemplate, TTo> migration)
            where TFrom : class
            where TFromTemplate : class
            where TTo : class
        {
            Ensure.That(migration, "migration").IsNotNull();

            lock (_db.LockObject)
            {
                var migrator = new DbStructureSetMigrator(_db);
                migrator.Migrate(migration);
            }
        }

        protected virtual void OnClearCache()
        {
            _db.CacheProvider.NotifyOfPurgeAll();
            _db.SchemaManager.ClearCache();
        }

        protected virtual void OnClearCache(IStructureSchema structureSchema)
        {
            _db.CacheProvider.NotifyOfPurge(structureSchema);
            _db.SchemaManager.RemoveFromCache(structureSchema);
        }
    }
}