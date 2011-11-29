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
using SisoDb.Structures;

namespace SisoDb
{
    internal class StructureSetUpdater<TOld, TNew>
        where TOld : class
        where TNew : class
    {
        protected const int MaxKeepQueueSize = 100;

        private IStructureId _deleteIdentityIdFrom;
        private IStructureId _deleteIdentityIdTo;
        private readonly IJsonSerializer _jsonSerializer;

        protected ISisoProviderFactory ProviderFactory { get; private set; }

        protected Queue<TNew> KeepQueue { get; private set; }

        protected ISisoConnectionInfo ConnectionInfo { get; private set; }

        protected IStructureSchema StructureSchemaOld { get; private set; }

        protected IStructureSchema StructureSchemaNew { get; private set; }

        protected IStructureBuilder StructureBuilder { get; private set; }

        internal StructureSetUpdater(ISisoConnectionInfo connectionInfo, IStructureSchema structureSchemaOld, IStructureSchema structureSchemaNew, IStructureBuilders structureBuilders)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();
            Ensure.That(structureSchemaOld, "structureSchemaOld").IsNotNull();
            Ensure.That(structureSchemaNew, "structureSchemaNew").IsNotNull();
            Ensure.That(structureBuilders, "structureBuilders").IsNotNull();

            ConnectionInfo = connectionInfo;
            
            StructureSchemaOld = structureSchemaOld;
            StructureSchemaNew = structureSchemaNew;
            if (StructureSchemaNew.IdAccessor.IdType != StructureSchemaOld.IdAccessor.IdType)
                throw new SisoDbException(ExceptionMessages.StructureSetUpdater_MissmatchInIdTypes);
            
            StructureBuilder = structureBuilders.ForUpdates(StructureSchemaNew);
            //StructureBuilder.Options.StructureIdStrategy = new KeepStructureIdStrategy();

            ProviderFactory = SisoEnvironment.ProviderFactories.Get(connectionInfo.ProviderType);
            _jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();
            KeepQueue = new Queue<TNew>(MaxKeepQueueSize);
        }

        public void Process(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
        {
            using (var dbClient = ProviderFactory.GetTransactionalDbClient(ConnectionInfo))
            {
                UpsertSchema(dbClient);

                if (ItterateStructures(dbClient, onProcess))
                    dbClient.Flush();

                dbClient.RefreshIndexes(StructureSchemaNew);
            }
        }

        private void UpsertSchema(IDbClient dbClient)
        {
            var upserter = ProviderFactory.GetDbSchemaUpserter(dbClient);
            upserter.Upsert(StructureSchemaNew);
        }

        private bool ItterateStructures(IDbClient dbClient, Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
        {
            using (var dbClientNonTrans = ProviderFactory.GetNonTransactionalDbClient(ConnectionInfo))
            {
                foreach (var json in dbClientNonTrans.GetJson(StructureSchemaOld))
                {
                    var oldStructure = _jsonSerializer.Deserialize<TOld>(json);
                    var oldId = GetStructureId(oldStructure);
                    if (oldId == null)
                        throw new SisoDbException(ExceptionMessages.SqlStructureSetUpdater_OldIdDoesNotExist);

                    var newStructure = _jsonSerializer.Deserialize<TNew>(json);

                    var status = onProcess(oldStructure, newStructure);

                    var newId = GetStructureId(newStructure);
                    if (newId == null)
                        throw new SisoDbException(ExceptionMessages.StructureSetUpdater_NewIdDoesNotExist);

                    if (!newId.Value.Equals(oldId.Value))
                        throw new SisoDbException(ExceptionMessages.StructureSetUpdater_NewIdDoesNotMatchOldId
                            .Inject(newId.Value, oldId.Value));

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
                        ProcessTrashQueue(dbClient);
                        ProcessKeepQueue(dbClient);
                    }
                }
            }

            ProcessTrashQueue(dbClient);
            ProcessKeepQueue(dbClient);

            return true;
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

        private void ProcessKeepQueue(IDbClient dbClient)
        {
            if (KeepQueue.Count < 1)
                return;

            var structures = new IStructure[KeepQueue.Count];
            var i = 0;
            while (KeepQueue.Count > 0)
            {
                var structureToKeep = KeepQueue.Dequeue();
                var structureItem = StructureBuilder.CreateStructure(structureToKeep, StructureSchemaNew);
                structures[i++] = structureItem;
            }
            var bulkInserter = ProviderFactory.GetDbStructureInserter(dbClient);
            bulkInserter.Insert(StructureSchemaNew, structures);
        }

        protected virtual void OnTrash(IStructureId structureId)
        {
            _deleteIdentityIdFrom = _deleteIdentityIdFrom ?? structureId;
            _deleteIdentityIdTo = structureId;
        }

        private void ProcessTrashQueue(IDbClient dbClient)
        {
            if (_deleteIdentityIdFrom == null)
                return;

            dbClient.DeleteWhereIdIsBetween(_deleteIdentityIdFrom, _deleteIdentityIdTo, StructureSchemaOld);

            _deleteIdentityIdFrom = null;
            _deleteIdentityIdTo = null;
        }
    }
}