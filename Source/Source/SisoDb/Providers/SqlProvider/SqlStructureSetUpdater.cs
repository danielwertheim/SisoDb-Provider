using System;
using System.Collections.Generic;
using System.Data;
using SisoDb.Providers.SqlProvider.BulkInserts;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    public class SqlStructureSetUpdater<TOld, TNew>
        where TOld : class
        where TNew : class
    {
        protected const int MaxKeepQueueSize = 500;

        protected Queue<TNew> KeepQueue { get; private set; }

        protected IProperty IdPropertyOld { get; private set; }

        protected IProperty IdPropertyNew { get; private set; }

        protected ISisoConnectionInfo ConnectionInfo { get; private set; }

        protected IStructureSchema StructureSchemaOld { get; private set; }

        protected IStructureSchema StructureSchemaNew { get; private set; }

        protected IStructureBuilder StructureBuilder { get; private set; }

        public SqlStructureSetUpdater(
            ISisoConnectionInfo connectionInfo, 
            IStructureSchema structureSchemaOld, IStructureSchema structureSchemaNew,
            IStructureBuilder structureBuilder)
        {
            ConnectionInfo = connectionInfo.AssertNotNull("connectionInfo");
            StructureSchemaOld = structureSchemaOld.AssertNotNull("structureSchemaOld");
            StructureSchemaNew = structureSchemaNew.AssertNotNull("structureSchemaNew");
            StructureBuilder = structureBuilder.AssertNotNull("structureBuilder");

            IdPropertyOld = StructureTypeInfo<TOld>.IdProperty;
            IdPropertyNew = StructureTypeInfo<TNew>.IdProperty;

            KeepQueue = new Queue<TNew>(MaxKeepQueueSize);
        }

        public void Process(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
        {
            using (var dbClient = new SqlDbClient(ConnectionInfo, true))
            {
                UpsertSchema(dbClient);

                if (ItterateStructures(dbClient, onProcess))
                    dbClient.Flush();
            }
        }

        private void UpsertSchema(ISqlDbClient dbClient)
        {
            var upserter = new SqlDbSchemaUpserter(dbClient);
            upserter.Upsert(StructureSchemaNew);
        }

        private bool ItterateStructures(ISqlDbClient dbClient, Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
        {
            foreach (var json in GetAllJson())
            {
                var oldStructure = StructureBuilder.JsonSerializer.ToItemOrNull<TOld>(json);
                var newStructure = StructureBuilder.JsonSerializer.ToItemOrNull<TNew>(json);
                var oldId = GetStructureId(oldStructure);
                if (oldId == null)
                    throw new SisoDbException(ExceptionMessages.SqlStructureSetUpdater_OldIdDoesNotExist);

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
            using (var dbClient = new SqlDbClient(ConnectionInfo, false))
            {
                var sql = dbClient.SqlStringsRepository.GetSql("GetAllById").Inject(StructureSchemaOld.GetStructureTableName());
                using (var cmd = dbClient.CreateCommand(CommandType.Text, sql))
                {
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                        {
                            yield return reader.GetString(0);
                        }
                    }
                }
            }
        }

        private IStructureId GetStructureId(object item)
        {
            var idProperty = item is TOld ? IdPropertyOld : IdPropertyNew;
            var structureSchema = item is TOld ? StructureSchemaOld : StructureSchemaNew;

            if (structureSchema.IdAccessor.IdType == IdTypes.Identity)
            {
                var idValue = idProperty.GetIdValue<int>(item);

                return idValue.HasValue ? StructureId.NewIdentityId(idValue.Value) : null;
            }

            if (structureSchema.IdAccessor.IdType == IdTypes.Guid)
            {
                var idValue = idProperty.GetIdValue<Guid>(item);

                return idValue.HasValue ? StructureId.NewGuidId(idValue.Value) : null;
            }

            return null;
        }

        protected virtual void OnKeep(TNew newStructure)
        {
            KeepQueue.Enqueue(newStructure);
        }

        protected virtual void DequeueStructuresToKeep(ISqlDbClient dbClient)
        {
            if (KeepQueue.Count < 1)
                return;

            var structures = new List<IStructure>(KeepQueue.Count);
            while (KeepQueue.Count > 0)
            {
                var structureToKeep = KeepQueue.Dequeue();
                var structureItem = StructureBuilder.CreateStructure(structureToKeep, StructureSchemaNew);
                structures.Add(structureItem);
            }
            var bulkInserter = new SqlBulkInserter(dbClient);
            bulkInserter.Insert(StructureSchemaNew, structures);
        }

        protected virtual void OnTrash(IStructureId structureId)
        {
            _deleteStructureIdFrom = _deleteStructureIdFrom ?? structureId;
            _deleteStructureIdTo = structureId;
        }

        private IStructureId _deleteStructureIdFrom;
        private IStructureId _deleteStructureIdTo;

        protected virtual void DequeueStructuresToTrash(ISqlDbClient dbClient)
        {
            if (_deleteStructureIdFrom == null)
                return;
            
            dbClient.DeleteWhereIdIsBetween(
              _deleteStructureIdFrom.Value, _deleteStructureIdTo.Value,
              StructureSchemaOld.GetStructureTableName(),
              StructureSchemaOld.GetIndexesTableName(),
              StructureSchemaOld.GetUniquesTableName());

            _deleteStructureIdFrom = null;
            _deleteStructureIdTo = null;
        }
    }
}