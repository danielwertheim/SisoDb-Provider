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

        public virtual void Migrate<TFrom, TTo>(Migration<TFrom, TTo> migration)
            where TFrom : class
            where TTo : class
        {
            Migrate(new Migration<TFrom, TFrom, TTo>(migration.Modifier));
        }

        public void Migrate<TFrom, TFromTemplate, TTo>(Migration<TFrom, TFromTemplate, TTo> migration)
            where TFrom : class
            where TFromTemplate : class
            where TTo : class
        {
            Ensure.That(migration, "migration").IsNotNull();

            Func<IDbClient, bool> onMigrationOfX;
            Action onCleanup;
            var structuresHasSameType = migration.From.Equals(migration.To);
            var structuresHasSameName = structuresHasSameType || string.Equals(migration.From.Name, migration.To.Name, StringComparison.OrdinalIgnoreCase);
            var fromTypeAndFromTemplateHasSameType = migration.From.Equals(migration.FromTemplate);

            IStructureSchema structureSchemaFrom, structureSchemaFromTemplate, structureSchemaTo;

            if (structuresHasSameType)
            {
                structureSchemaFrom = Db.StructureSchemas.GetSchema(migration.To);
                structureSchemaFromTemplate = fromTypeAndFromTemplateHasSameType
                    ? structureSchemaFrom
                    : Db.StructureSchemas.GetSchema(migration.FromTemplate);
                structureSchemaTo = structureSchemaFrom;
                onMigrationOfX = dbClient => OnMigrationOfSameStructureSet<TFrom, TFromTemplate, TTo>(structureSchemaFrom, structureSchemaFromTemplate, structureSchemaTo, dbClient, migration.Modifier);
                onCleanup = () => Db.CacheProvider.NotifyOfPurge(structureSchemaFrom);
            }
            else
            {
                structureSchemaFrom = Db.StructureSchemas.GetSchema(migration.From);
                structureSchemaFromTemplate = fromTypeAndFromTemplateHasSameType
                    ? structureSchemaFrom
                    : Db.StructureSchemas.GetSchema(migration.FromTemplate);
                structureSchemaTo = Db.StructureSchemas.GetSchema(migration.To);
                EnsureThatSchemasAreCompatible(structureSchemaFrom, structureSchemaTo);
                onCleanup = () =>
                {
                    Db.CacheProvider.NotifyOfPurge(structureSchemaFrom);
                    Db.CacheProvider.NotifyOfPurge(structureSchemaTo);
                };

                if (structuresHasSameName)
                    onMigrationOfX = dbClient =>
                        OnMigrationOfSameStructureSet<TFrom, TFromTemplate, TTo>(structureSchemaFrom, structureSchemaFromTemplate, structureSchemaTo, dbClient, migration.Modifier);
                else
                    onMigrationOfX = dbClient =>
                        OnMigrationOfDifferentStructureSets<TFrom, TFromTemplate, TTo>(structureSchemaFrom, structureSchemaFromTemplate, structureSchemaTo, dbClient, migration.Modifier);
            }

            OnMigrate(onMigrationOfX, onCleanup);
        }

        protected virtual void OnMigrate(Func<IDbClient, bool> onMigrationOfX, Action onCleanup)
        {
            using (var dbClient = Db.ProviderFactory.GetTransactionalDbClient(Db.ConnectionInfo))
            {
                try
                {
                    if (!onMigrationOfX.Invoke(dbClient))
                        dbClient.MarkAsFailed();
                }
                finally
                {
                    onCleanup.Invoke();
                }
            }
        }

        protected virtual bool OnMigrationOfDifferentStructureSets<TFrom, TFromTemplate, TTo>(IStructureSchema structureSchemaFrom, IStructureSchema structureSchemaFromTemplate, IStructureSchema structureSchemaTo, IDbClient dbClientTransactional, Func<TFromTemplate, TTo, MigrationStatuses> modifier)
            where TFrom : class
            where TFromTemplate : class
            where TTo : class
        {
            var maxKeepQueueSize = Db.Settings.MaxInsertManyBatchSize;
            var serializer = Db.Serializer;
            var keepQueue = new List<TTo>(maxKeepQueueSize);
            var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchemaTo);
            
            Func<string, TFromTemplate> fromDeserializer;
            if (structureSchemaFrom.Type.Type.Equals(structureSchemaFromTemplate.Type.Type))
                fromDeserializer = serializer.Deserialize<TFromTemplate>;
            else
                fromDeserializer = serializer.DeserializeAnonymous<TFromTemplate>;

            using (var dbClientNonTransactional = Db.ProviderFactory.GetNonTransactionalDbClient(Db.ConnectionInfo))
            {
                Db.SchemaManager.UpsertStructureSet(structureSchemaTo, dbClientNonTransactional);

                foreach (var json in dbClientNonTransactional.GetJsonOrderedByStructureId(structureSchemaFrom))
                {
                    var oldItem = fromDeserializer(json);
                    var oldId = GetOldStructureId(structureSchemaFromTemplate, oldItem);
                    var newItem = serializer.Deserialize<TTo>(json);

                    var modifierStatus = modifier.Invoke(oldItem, newItem);
                    if (modifierStatus == MigrationStatuses.Abort)
                        return false;

                    var newId = GetNewStructureId(structureSchemaTo, newItem);
                    EnsureThatNewIdEqualsOldId(oldId, newId);

                    if (modifierStatus == MigrationStatuses.Keep)
                        keepQueue.Add(newItem);

                    if (keepQueue.Count == maxKeepQueueSize)
                        ProcessKeepQueue(keepQueue, structureSchemaTo, dbClientTransactional, structureBuilder);
                }
            }

            ProcessKeepQueue(keepQueue, structureSchemaTo, dbClientTransactional, structureBuilder);

            return true;
        }

        protected virtual bool OnMigrationOfSameStructureSet<TFrom, TFromTemplate, TTo>(IStructureSchema structureSchemaFrom, IStructureSchema structureSchemaFromTemplate, IStructureSchema structureSchemaTo, IDbClient dbClientTransactional, Func<TFromTemplate, TTo, MigrationStatuses> modifier)
            where TFrom : class
            where TFromTemplate : class
            where TTo : class
        {
            var maxKeepQueueSize = Db.Settings.MaxInsertManyBatchSize;
            var serializer = Db.Serializer;
            var keepQueue = new List<TTo>(maxKeepQueueSize);
            var trashQueue = new List<IStructureId>(maxKeepQueueSize);
            var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchemaTo);

            Func<string, TFromTemplate> fromDeserializer;
            if (structureSchemaFrom.Type.Type.Equals(structureSchemaFromTemplate.Type.Type))
                fromDeserializer = serializer.Deserialize<TFromTemplate>;
            else
                fromDeserializer = serializer.DeserializeAnonymous<TFromTemplate>;

            using (var dbClientNonTransactional = Db.ProviderFactory.GetNonTransactionalDbClient(Db.ConnectionInfo))
            {
                Db.SchemaManager.UpsertStructureSet(structureSchemaTo, dbClientNonTransactional);

                foreach (var json in dbClientNonTransactional.GetJsonOrderedByStructureId(structureSchemaTo))
                {
                    var oldItem = fromDeserializer(json);
                    var oldId = GetOldStructureId(structureSchemaFromTemplate, oldItem);
                    var newItem = serializer.Deserialize<TTo>(json);

                    var modifierStatus = modifier.Invoke(oldItem, newItem);
                    if (modifierStatus == MigrationStatuses.Abort)
                        return false;

                    var newId = GetNewStructureId(structureSchemaTo, newItem);
                    EnsureThatNewIdEqualsOldId(oldId, newId);

                    if (modifierStatus == MigrationStatuses.Keep)
                        keepQueue.Add(newItem);

                    trashQueue.Add(newId);

                    if (keepQueue.Count == maxKeepQueueSize)
                    {
                        ProcessTrashQueue(trashQueue, structureSchemaTo, dbClientTransactional);
                        ProcessKeepQueue(keepQueue, structureSchemaTo, dbClientTransactional, structureBuilder);
                    }
                }
            }

            ProcessTrashQueue(trashQueue, structureSchemaTo, dbClientTransactional);
            ProcessKeepQueue(keepQueue, structureSchemaTo, dbClientTransactional, structureBuilder);

            return true;
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

        protected void ProcessTrashQueue(IList<IStructureId> structureIds, IStructureSchema structureSchema, IDbClient dbClient)
        {
            if (!structureIds.Any())
                return;

            dbClient.DeleteByIds(structureIds.ToArray(), structureSchema);
            structureIds.Clear();
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

        protected void EnsureThatSchemasAreCompatible(IStructureSchema structureSchemaFrom, IStructureSchema structureSchemaTo)
        {
            Ensure.That(structureSchemaFrom, "structureSchemaFrom").IsNotNull();
            Ensure.That(structureSchemaTo, "structureSchemaTo").IsNotNull();

            if (structureSchemaTo.IdAccessor.IdType != structureSchemaFrom.IdAccessor.IdType)
                throw new SisoDbException(ExceptionMessages.StructureSetMigrator_MissmatchInIdTypes);
        }

        protected void EnsureThatNewIdEqualsOldId(IStructureId oldId, IStructureId newId)
        {
            if (!newId.Value.Equals(oldId.Value))
                throw new SisoDbException(ExceptionMessages.StructureSetMigrator_NewIdDoesNotMatchOldId.Inject(newId.Value, oldId.Value));
        }
    }
}