using System;
using System.Collections.Generic;
using System.Data;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Providers;
using SisoDb.Resources;
using SisoDb.Sql2008.Dac;
using SisoDb.Sql2008.DbSchema;

namespace SisoDb.Sql2008
{
    public class Sql2008StructureSetUpdater<TOld, TNew>
        where TOld : class
        where TNew : class
    {
        protected const int MaxKeepQueueSize = 500;

        private IStructureId _deleteIdFrom;
        private IStructureId _deleteIdTo;

        protected Queue<TNew> KeepQueue { get; private set; }

        protected SqlConnectionInfo ConnectionInfo { get; private set; }

        protected IStructureSchema StructureSchemaOld { get; private set; }

        protected IStructureSchema StructureSchemaNew { get; private set; }

        protected IStructureBuilder StructureBuilder { get; private set; }

        public Sql2008StructureSetUpdater(
            SqlConnectionInfo connectionInfo, 
            IStructureSchema structureSchemaOld, IStructureSchema structureSchemaNew,
            IStructureBuilder structureBuilder)
        {
            Ensure.That(() => connectionInfo).IsNotNull();
            Ensure.That(() => structureSchemaOld).IsNotNull();
            Ensure.That(() => structureSchemaNew).IsNotNull();
            Ensure.That(() => structureBuilder).IsNotNull();

            ConnectionInfo = connectionInfo;
            StructureSchemaOld = structureSchemaOld;
            StructureSchemaNew = structureSchemaNew;
            StructureBuilder = structureBuilder;

            KeepQueue = new Queue<TNew>(MaxKeepQueueSize);
        }

        public void Process(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
        {
            using (var dbClient = new Sql2008DbClient(ConnectionInfo, true))
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

        private void UpsertSchema(Sql2008DbClient dbClient)
        {
            var upserter = new SqlDbSchemaUpserter(dbClient);
            upserter.Upsert(StructureSchemaNew);
        }

        private bool ItterateStructures(Sql2008DbClient dbClient, Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
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
            using (var dbClient = new Sql2008DbClient(ConnectionInfo, false))
            {
                var sql = dbClient.SqlStatements.GetSql("GetAllById").Inject(StructureSchemaOld.GetStructureTableName());
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

        private IStructureId GetSisoId<T>(T item)
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

        protected virtual void DequeueStructuresToKeep(Sql2008DbClient dbClient)
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

        protected virtual void OnTrash(IStructureId sisoId)
        {
            _deleteIdFrom = _deleteIdFrom ?? sisoId;
            _deleteIdTo = sisoId;
        }

        protected virtual void DequeueStructuresToTrash(Sql2008DbClient dbClient)
        {
            if (_deleteIdFrom == null)
                return;
            
            dbClient.DeleteWhereIdIsBetween(
              _deleteIdFrom.Value, _deleteIdTo.Value,
              StructureSchemaOld.GetStructureTableName(),
              StructureSchemaOld.GetIndexesTableName(),
              StructureSchemaOld.GetUniquesTableName());

            _deleteIdFrom = null;
            _deleteIdTo = null;
        }
    }
}