using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Resources;

namespace SisoDb
{
    public class DbStructureSetMigrator : IStructureSetMigrator
    {
        protected readonly ISisoDbDatabase Db;
        protected int MaxKeepQueueSize;

        public DbStructureSetMigrator(ISisoDbDatabase db)
        {
            Ensure.That(db, "db").IsNotNull();

            Db = db;
            MaxKeepQueueSize = 500;
        }

        public virtual void Migrate<TOld, TNew>(Func<TOld, TNew, StructureSetMigratorStatuses> modifier)
            where TOld : class
            where TNew : class
        {
            var oldType = typeof(TOld);
            var newType = typeof(TNew);
            EnsureThatTypesAreNotTheSame(newType, oldType);

            var structureSchemaOld = Db.StructureSchemas.GetSchema(oldType);
            var structureSchemaNew = Db.StructureSchemas.GetSchema(newType);
            EnsureSchemasAreCompatible(structureSchemaOld, structureSchemaNew);

            var isUpdatingSameSchema = structureSchemaOld.Name.Equals(structureSchemaNew.Name, StringComparison.OrdinalIgnoreCase);
            if (isUpdatingSameSchema)
            {
                Db.StructureSchemas.RemoveSchema(oldType);
                Db.SchemaManager.RemoveFromCache(structureSchemaOld);
            }

            using (var t = Db.ProviderFactory.GetRequiredTransaction())
            {
                using (var dbClient = Db.ProviderFactory.GetDbClient(Db.ConnectionInfo))
                {
                    try
                    {
                        Db.SchemaManager.UpsertStructureSet(structureSchemaNew, dbClient);

                        if (!OnUpdate(structureSchemaOld, structureSchemaNew, dbClient, modifier))
                            t.MarkAsFailed();
                    }
                    finally
                    {
                        if (!isUpdatingSameSchema)
                        {
                            Db.StructureSchemas.RemoveSchema(oldType);
                            Db.SchemaManager.DropStructureSet(structureSchemaOld, dbClient);
                        }
                    }
                }
            }
        }

        protected void EnsureThatTypesAreNotTheSame(Type newType, Type oldType)
        {
            if (oldType == newType)
                throw new SisoDbException(ExceptionMessages.StructureSetUpdater_TOld_TNew_SameType);
        }

        protected void EnsureSchemasAreCompatible(IStructureSchema structureSchemaOld, IStructureSchema structureSchemaNew)
        {
            Ensure.That(structureSchemaOld, "structureSchemaOld").IsNotNull();
            Ensure.That(structureSchemaNew, "structureSchemaNew").IsNotNull();

            if (structureSchemaNew.IdAccessor.IdType != structureSchemaOld.IdAccessor.IdType)
                throw new SisoDbException(ExceptionMessages.StructureSetUpdater_MissmatchInIdTypes);
        }

        protected virtual bool OnUpdate<TOld, TNew>(IStructureSchema structureSchemaOld, IStructureSchema structureSchemaNew, IDbClient dbClientTransactional, Func<TOld, TNew, StructureSetMigratorStatuses> modifier)
            where TOld : class
            where TNew : class
        {
            var serializer = Db.Serializer;
            var keepQueue = new List<TNew>();
            var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchemaNew);
            var deleteIdInterval = new StructureIdInterval();
            
            using (var dbClient = GetNonTransactionalDbClient())
            {
                foreach (var json in dbClient.GetJsonOrderedByStructureId(structureSchemaOld))
                {
                    var oldItem = serializer.Deserialize<TOld>(json);
                    var oldId = GetOldStructureId(structureSchemaOld, oldItem);

                    var newItem = serializer.Deserialize<TNew>(json);

                    var modifierStatus = modifier.Invoke(oldItem, newItem);
                    if (modifierStatus == StructureSetMigratorStatuses.Abort)
                        return false;

                    var newId = GetNewStructureId(structureSchemaNew, newItem);
                    EnsureNewIdEqualsOldId(oldId, newId);

                    switch (modifierStatus)
                    {
                        case StructureSetMigratorStatuses.Keep:
                            deleteIdInterval.Set(oldId);
                            keepQueue.Add(newItem);
                            break;
                        case StructureSetMigratorStatuses.Trash:
                            deleteIdInterval.Set(oldId);
                            break;
                    }

                    if (keepQueue.Count == MaxKeepQueueSize)
                    {
                        ProcessTrashQueue(deleteIdInterval, structureSchemaOld, dbClientTransactional);
                        ProcessKeepQueue(keepQueue, structureSchemaNew, dbClientTransactional, structureBuilder);
                    }
                }
            }

            ProcessTrashQueue(deleteIdInterval, structureSchemaOld, dbClientTransactional);
            ProcessKeepQueue(keepQueue, structureSchemaNew, dbClientTransactional, structureBuilder);

            return true;
        }

        private IDbClient GetNonTransactionalDbClient()
        {
            using (Db.ProviderFactory.GetSuppressedTransaction())
            {
                return Db.ProviderFactory.GetDbClient(Db.ConnectionInfo);
            }
        }

        protected IStructureId GetOldStructureId<T>(IStructureSchema structureSchema, T oldStructure) where T : class
        {
            var id = structureSchema.IdAccessor.GetValue(oldStructure);
            if (id == null)
                throw new SisoDbException(ExceptionMessages.SqlStructureSetUpdater_OldIdDoesNotExist);

            return id;
        }

        protected IStructureId GetNewStructureId<T>(IStructureSchema structureSchema, T oldStructure) where T : class
        {
            var id = structureSchema.IdAccessor.GetValue(oldStructure);
            if (id == null)
                throw new SisoDbException(ExceptionMessages.StructureSetUpdater_NewIdDoesNotExist);

            return id;
        }

        protected void EnsureNewIdEqualsOldId(IStructureId oldId, IStructureId newId)
        {
            if (!newId.Value.Equals(oldId.Value))
                throw new SisoDbException(ExceptionMessages.StructureSetUpdater_NewIdDoesNotMatchOldId.Inject(newId.Value, oldId.Value));
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

        protected void ProcessTrashQueue(StructureIdInterval deleteIdInterval, IStructureSchema structureSchema, IDbClient dbClient)
        {
            if (!deleteIdInterval.IsComplete)
                return;

            dbClient.DeleteWhereIdIsBetween(deleteIdInterval.From, deleteIdInterval.To, structureSchema);

            deleteIdInterval.Clear();
        }
    }
}