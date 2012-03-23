using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Caching;
using SisoDb.Dac;
using SisoDb.Resources;

namespace SisoDb.Maintenance
{
    public class DbStructureSetMigrator
    {
        protected readonly ISisoDatabase Db;

        public DbStructureSetMigrator(ISisoDatabase db)
        {
            Ensure.That(db, "db").IsNotNull();

            Db = db;
        }

        public virtual void Migrate<TOld, TNew>(Func<TOld, TNew, MigrationStatuses> modifier)
            where TOld : class
            where TNew : class
        {
            Ensure.That(modifier, "modifier").IsNotNull();

            var oldType = typeof(TOld);
            var newType = typeof(TNew);
            EnsureThatTypesAreNotTheSame(oldType, newType);
            EnsureThatTypesHasDifferentNames(oldType, newType);

            var structureSchemaOld = Db.StructureSchemas.GetSchema(oldType);
            var structureSchemaNew = Db.StructureSchemas.GetSchema(newType);
            EnsureThatSchemasAreCompatible(structureSchemaOld, structureSchemaNew);

            using (var dbClient = Db.ProviderFactory.GetTransactionalDbClient(Db.ConnectionInfo))
            {
                try
                {
                    if (!OnMigrate(structureSchemaOld, structureSchemaNew, dbClient, modifier))
                        dbClient.MarkAsFailed();
                }
                finally
                {
                    Db.CacheProvider.NotifyOfPurge(structureSchemaNew);
                    Db.CacheProvider.NotifyOfPurge(structureSchemaOld);
                }
            }
        }

        protected void EnsureThatTypesAreNotTheSame(Type oldType, Type newType)
        {
            if (oldType == newType)
                throw new SisoDbException(ExceptionMessages.StructureSetMigrator_TOld_TNew_SameType);
        }

        protected void EnsureThatTypesHasDifferentNames(Type oldStructureSchema, Type newStructureSchema)
        {
            if(string.Equals(oldStructureSchema.Name, newStructureSchema.Name, StringComparison.OrdinalIgnoreCase))
                throw new SisoDbException("Migrate<TOld, TNew> can not be used for structures with the same name.");
        }

        protected void EnsureThatSchemasAreCompatible(IStructureSchema structureSchemaOld, IStructureSchema structureSchemaNew)
        {
            Ensure.That(structureSchemaOld, "structureSchemaOld").IsNotNull();
            Ensure.That(structureSchemaNew, "structureSchemaNew").IsNotNull();

            if (structureSchemaNew.IdAccessor.IdType != structureSchemaOld.IdAccessor.IdType)
                throw new SisoDbException(ExceptionMessages.StructureSetMigrator_MissmatchInIdTypes);
        }

        protected virtual bool OnMigrate<TOld, TNew>(IStructureSchema structureSchemaOld, IStructureSchema structureSchemaNew, IDbClient dbClientTransactional, Func<TOld, TNew, MigrationStatuses> modifier)
            where TOld : class
            where TNew : class
        {
            var maxKeepQueueSize = Db.Settings.MaxInsertManyBatchSize;
            var serializer = Db.Serializer;
            var keepQueue = new List<TNew>(maxKeepQueueSize);
            var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchemaNew);

            using (var dbClientNonTransactional = Db.ProviderFactory.GetNonTransactionalDbClient(Db.ConnectionInfo))
            {
                Db.SchemaManager.UpsertStructureSet(structureSchemaNew, dbClientNonTransactional);

                foreach (var json in dbClientNonTransactional.GetJsonOrderedByStructureId(structureSchemaOld))
                {
                    var oldItem = serializer.Deserialize<TOld>(json);
                    var oldId = GetOldStructureId(structureSchemaOld, oldItem);
                    var newItem = serializer.Deserialize<TNew>(json);

                    var modifierStatus = modifier.Invoke(oldItem, newItem);
                    if (modifierStatus == MigrationStatuses.Abort)
                        return false;

                    var newId = GetNewStructureId(structureSchemaNew, newItem);
                    EnsureThatNewIdEqualsOldId(oldId, newId);

                    if(modifierStatus == MigrationStatuses.Keep)
                        keepQueue.Add(newItem);

                    if (keepQueue.Count == maxKeepQueueSize)
                        ProcessKeepQueue(keepQueue, structureSchemaNew, dbClientTransactional, structureBuilder);
                }
            }

            ProcessKeepQueue(keepQueue, structureSchemaNew, dbClientTransactional, structureBuilder);

            return true;
        }

        protected IStructureId GetOldStructureId<T>(IStructureSchema structureSchema, T oldStructure) where T : class
        {
            var id = structureSchema.IdAccessor.GetValue(oldStructure);
            if (id == null)
                throw new SisoDbException(ExceptionMessages.StructureSetMigrator_OldIdDoesNotExist);

            return id;
        }

        protected IStructureId GetNewStructureId<T>(IStructureSchema structureSchema, T oldStructure) where T : class
        {
            var id = structureSchema.IdAccessor.GetValue(oldStructure);
            if (id == null)
                throw new SisoDbException(ExceptionMessages.StructureSetMigrator_NewIdDoesNotExist);

            return id;
        }

        protected void EnsureThatNewIdEqualsOldId(IStructureId oldId, IStructureId newId)
        {
            if (!newId.Value.Equals(oldId.Value))
                throw new SisoDbException(ExceptionMessages.StructureSetMigrator_NewIdDoesNotMatchOldId.Inject(newId.Value, oldId.Value));
        }

        protected void ProcessKeepQueue<T>(IList<T> keepQueue, IStructureSchema structureSchema, IDbClient dbClient, IStructureBuilder structureBuilder) where T : class
        {
            if (keepQueue.Count < 1)
                return;

            var structures = structureBuilder.CreateStructures(keepQueue.ToArray(), structureSchema);
            keepQueue.Clear();

            if (structures.Length == 0)
                return;

            var bulkInserter = Db.ProviderFactory.GetStructureInserter(dbClient);
            bulkInserter.Insert(structureSchema, structures);
        }
    }
}