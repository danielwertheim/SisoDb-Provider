using System;
using System.Collections.Generic;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Providers;
using SisoDb.Resources;
using SisoDb.Serialization;

namespace SisoDb
{
    public class SqlStructureSetUpdater<TOld, TNew>
        where TOld : class
        where TNew : class
    {
        protected const int MaxKeepQueueSize = 500;

        private IStructureId _deleteIdFrom;
        private IStructureId _deleteIdTo;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly StructureBuilderOptions _structureBuilderOptions;

        protected ISisoProviderFactory ProviderFactory { get; private set; }

        protected Queue<TNew> KeepQueue { get; private set; }

        protected ISisoConnectionInfo ConnectionInfo { get; private set; }

        protected IStructureSchema StructureSchemaOld { get; private set; }

        protected IStructureSchema StructureSchemaNew { get; private set; }

        protected IStructureBuilder StructureBuilder { get; private set; }

        public SqlStructureSetUpdater(ISisoConnectionInfo connectionInfo, IStructureSchema structureSchemaOld, IStructureSchema structureSchemaNew, IStructureBuilder structureBuilder)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();
            Ensure.That(structureSchemaOld, "structureSchemaOld").IsNotNull();
            Ensure.That(structureSchemaNew, "structureSchemaNew").IsNotNull();
            Ensure.That(structureBuilder, "structureBuilder").IsNotNull();

            ConnectionInfo = connectionInfo;
            StructureSchemaOld = structureSchemaOld;
            StructureSchemaNew = structureSchemaNew;
            StructureBuilder = structureBuilder;

            ProviderFactory = SisoEnvironment.ProviderFactories.Get(connectionInfo.ProviderType);
            _jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();
            _structureBuilderOptions = new StructureBuilderOptions
            {
                Serializer = new SerializerForStructureBuilder(),
                KeepStructureId = true
            };
            KeepQueue = new Queue<TNew>(MaxKeepQueueSize);
        }

        public void Process(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
        {
            using (var dbClient = ProviderFactory.GetDbClient(ConnectionInfo, true))
            {
                UpsertSchema(dbClient);

                if (ItterateStructures(dbClient, onProcess))
                    dbClient.Flush();

                dbClient.RebuildIndexes(StructureSchemaNew);
            }
        }

        private void UpsertSchema(IDbClient dbClient)
        {
            var upserter = ProviderFactory.GetDbSchemaUpserter(dbClient);
            upserter.Upsert(StructureSchemaNew);
        }

        private bool ItterateStructures(IDbClient dbClient, Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
        {
            foreach (var json in GetAllJson())
            {
                var oldStructure = _jsonSerializer.Deserialize<TOld>(json);
                var oldId = GetStructureId(oldStructure);
                if (oldId == null)
                    throw new SisoDbException(ExceptionMessages.SqlStructureSetUpdater_OldIdDoesNotExist);

                var newStructure = _jsonSerializer.Deserialize<TNew>(json);

                var status = onProcess(oldStructure, newStructure);

                var newId = GetStructureId(newStructure);
                if (newId == null)
                    throw new SisoDbException(ExceptionMessages.SqlStructureSetUpdater_NewIdDoesNotExist);

                if (!newId.Value.Equals(oldId.Value))
                    throw new SisoDbException(
                        ExceptionMessages.SqlStructureSetUpdater_NewIdDoesNotMatchOldId.Inject(newId.Value, oldId.Value));

                switch (status)
                {
                    case StructureSetUpdaterStatuses.Keep:
                        OnTrash(newId);
                        OnKeep(newStructure);
                        break;
                    case StructureSetUpdaterStatuses.Trash:
                        OnTrash(newId);
                        break;
                    case StructureSetUpdaterStatuses.Abort:
                        return false;
                }

                if (KeepQueue.Count == MaxKeepQueueSize)
                {
                    DequeueStructuresToTrash(dbClient);
                    DequeueStructuresToKeep(dbClient);
                }
            }

            DequeueStructuresToTrash(dbClient);
            DequeueStructuresToKeep(dbClient);

            return true;
        }

        private IEnumerable<string> GetAllJson()
        {
            using (var dbClient = ProviderFactory.GetDbClient(ConnectionInfo, false))
            {
                return dbClient.GetJson(StructureSchemaOld);
            }
        }

        private IStructureId GetStructureId<T>(T item)
            where T : class
        {
            var structureSchema = item is TOld ? StructureSchemaOld : StructureSchemaNew;

            return structureSchema.IdAccessor.GetValue(item);
        }

        protected virtual void OnKeep(TNew newStructure)
        {
            KeepQueue.Enqueue(newStructure);
        }

        protected virtual void DequeueStructuresToKeep(IDbClient dbClient)
        {
            if (KeepQueue.Count < 1)
                return;

            var structures = new List<IStructure>(KeepQueue.Count);
            while (KeepQueue.Count > 0)
            {
                var structureToKeep = KeepQueue.Dequeue();
                var structureItem = StructureBuilder.CreateStructure(structureToKeep, StructureSchemaNew, _structureBuilderOptions);
                structures.Add(structureItem);
            }
            var bulkInserter = ProviderFactory.GetDbBulkInserter(dbClient);
            bulkInserter.Insert(StructureSchemaNew, structures);
        }

        protected virtual void OnTrash(IStructureId structureId)
        {
            _deleteIdFrom = _deleteIdFrom ?? structureId;
            _deleteIdTo = structureId;
        }

        protected virtual void DequeueStructuresToTrash(IDbClient dbClient)
        {
            if (_deleteIdFrom == null)
                return;

            dbClient.DeleteWhereIdIsBetween(_deleteIdFrom.Value, _deleteIdTo.Value, StructureSchemaOld);

            _deleteIdFrom = null;
            _deleteIdTo = null;
        }
    }
}