using System;
using System.Collections.Generic;
using System.Data;
using SisoDb.Core;
using SisoDb.Providers.Sql2008Provider.BulkInserts;
using SisoDb.Providers.Sql2008Provider.DbSchema;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Sql2008Provider
{
    public class SqlStructureSetUpdater<TOld, TNew>
        where TOld : class
        where TNew : class
    {
        protected const int MaxKeepQueueSize = 500;

        private ISisoId _deleteSisoIdFrom;
        private ISisoId _deleteSisoIdTo;

        protected Queue<TNew> KeepQueue { get; private set; }

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

            KeepQueue = new Queue<TNew>(MaxKeepQueueSize);
        }

        public void Process(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
        {
            using (var dbClient = new SqlDbClient(ConnectionInfo, true))
            {
                UpsertSchema(dbClient);

                if (ItterateStructures(dbClient, onProcess))
                    dbClient.Flush();

                dbClient.RebuildIndexes(
                    StructureSchemaNew.GetStructureTableName(),
                    StructureSchemaNew.GetIndexesTableName(),
                    StructureSchemaNew.GetUniquesTableName());
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
                var oldId = GetSisoId(oldStructure);
                if (oldId == null)
                    throw new SisoDbException(ExceptionMessages.SqlStructureSetUpdater_OldIdDoesNotExist);

                var newStructure = StructureBuilder.JsonSerializer.ToItemOrNull<TNew>(json);
                
                var status = onProcess(oldStructure, newStructure);

                var newId = GetSisoId(newStructure);
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
                        reader.Close();
                    }
                }
            }
        }

        private ISisoId GetSisoId<T>(T item)
            where T : class 
        {
            var structureSchema = item is TOld ? StructureSchemaOld : StructureSchemaNew;

            if (structureSchema.IdAccessor.IdType == IdTypes.Identity)
            {
                var idValue = structureSchema.IdAccessor.GetValue<T, int>(item);

                return idValue.HasValue ? SisoId.NewIdentityId(idValue.Value) : null;
            }

            if (structureSchema.IdAccessor.IdType == IdTypes.Guid)
            {
                var idValue = structureSchema.IdAccessor.GetValue<T, Guid>(item);

                return idValue.HasValue ? SisoId.NewGuidId(idValue.Value) : null;
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

        protected virtual void OnTrash(ISisoId sisoId)
        {
            _deleteSisoIdFrom = _deleteSisoIdFrom ?? sisoId;
            _deleteSisoIdTo = sisoId;
        }

        protected virtual void DequeueStructuresToTrash(ISqlDbClient dbClient)
        {
            if (_deleteSisoIdFrom == null)
                return;
            
            dbClient.DeleteWhereIdIsBetween(
              _deleteSisoIdFrom.Value, _deleteSisoIdTo.Value,
              StructureSchemaOld.GetStructureTableName(),
              StructureSchemaOld.GetIndexesTableName(),
              StructureSchemaOld.GetUniquesTableName());

            _deleteSisoIdFrom = null;
            _deleteSisoIdTo = null;
        }
    }
}