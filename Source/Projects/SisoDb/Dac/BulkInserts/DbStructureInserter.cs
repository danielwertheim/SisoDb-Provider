using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCore.Collections;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Core;
using SisoDb.DbSchema;
using SisoDb.Structures;

namespace SisoDb.Dac.BulkInserts
{
    public class DbStructureInserter : IStructureInserter
    {
        protected class IndexInsertAction
        {
            public IStructureIndex[] Data;
            public Action<IStructureIndex[], IDbClient> Action;
            public bool HasData
            {
                get { return Data != null && Data.Length > 0; }
            }
        }

        protected static readonly Type TextType;
        protected const int MaxNumOfStructuresBeforeParallelEscalation = 10;

        protected readonly IDbClient MainDbClient;
        protected readonly Func<IDbClient> DbClientFnForParallelInserts;

        protected bool SupportsParallelInserts
        {
            get
            {
                return
                    MainDbClient.ConnectionInfo.ParallelInserts == ParallelInserts.On
                    && DbClientFnForParallelInserts != null;
            }
        }

        static DbStructureInserter()
        {
            TextType = typeof(Text);
        }

        public DbStructureInserter(IDbClient mainDbClient, Func<IDbClient> dbClientFnForParallelInserts = null)
        {
            MainDbClient = mainDbClient;
            DbClientFnForParallelInserts = dbClientFnForParallelInserts;
        }

        public virtual void Insert(IStructureSchema structureSchema, IStructure[] structures)
        {
            var groupedIndexInsertActions = new IndexInsertAction[0];

            Task task = null;
            try
            {
                task = Task.Factory.StartNew(() => groupedIndexInsertActions = CreateGroupedIndexInsertActions(structureSchema, structures));

                InsertStructures(structureSchema, structures);
                InsertUniques(structureSchema, structures);

                Task.WaitAll(task);
            }
            finally
            {
                if (task != null && task.Status == TaskStatus.RanToCompletion)
                    task.Dispose();
            }

            if (!SupportsParallelInserts || !groupedIndexInsertActions.Any() || structures.Length < MaxNumOfStructuresBeforeParallelEscalation)
            {
                InsertIndexes(groupedIndexInsertActions);
                return;
            }

            ParallelInsert(structureSchema, structures, groupedIndexInsertActions);
        }

        protected virtual void ParallelInsert(IStructureSchema structureSchema, IStructure[] structures, IndexInsertAction[] groupedIndexInsertActions)
        {
            if (!groupedIndexInsertActions.Any())
                return;

            var indexesDbClients = new IDbClient[groupedIndexInsertActions.Length];
            
            try
            {
                for (var c = 0; c < groupedIndexInsertActions.Length; c++)
                    indexesDbClients[c] = DbClientFnForParallelInserts.Invoke();

                Parallel.For(0, groupedIndexInsertActions.Length, new ParallelOptions { MaxDegreeOfParallelism = indexesDbClients.Length },
                    i =>
                    {
                        using (var indexesDbClient = indexesDbClients[i])
                        {
                            groupedIndexInsertActions[i].Action.Invoke(groupedIndexInsertActions[i].Data, indexesDbClient);
                        }
                        indexesDbClients[i] = null;
                    });
            }
            finally
            {
                for (var c = 0; c < indexesDbClients.Length; c++)
                {
                    Disposer.TryDispose(indexesDbClients[c]);
                    indexesDbClients[c] = null;
                }
            }
        }

        protected virtual void InsertStructures(IStructureSchema structureSchema, IStructure[] structures)
        {
            if (!structures.Any())
                return;

            if (structures.Length == 1)
                MainDbClient.SingleInsertStructure(structures[0], structureSchema);
            else
                BulkInsertStructures(structureSchema, structures);
        }

        protected virtual void BulkInsertStructures(IStructureSchema structureSchema, IStructure[] structures)
        {
            if (!structures.Any())
                return;

            var structureStorageSchema = new StructureStorageSchema(structureSchema, structureSchema.GetStructureTableName());

            using (var structuresReader = new StructuresReader(structureStorageSchema, structures))
            {
                using (var bulkInserter = MainDbClient.GetBulkCopy())
                {
                    bulkInserter.DestinationTableName = structuresReader.StorageSchema.Name;
                    bulkInserter.BatchSize = structures.Length;

                    foreach (var field in structuresReader.StorageSchema.GetFieldsOrderedByIndex())
                        bulkInserter.AddColumnMapping(field.Name, field.Name);

                    bulkInserter.Write(structuresReader);
                }
            }
        }

        protected virtual void InsertIndexes(IndexInsertAction[] groupedIndexInsertActions)
        {
            foreach (var groupedIndexInsertAction in groupedIndexInsertActions)
                groupedIndexInsertAction.Action.Invoke(groupedIndexInsertAction.Data, MainDbClient);
        }

        protected virtual void BulkInsertIndexes(IndexesReader indexesReader, IDbClient dbClient)
        {
            using (indexesReader)
            {
                if (indexesReader.RecordsAffected < 1)
                    return;

                using (var bulkInserter = dbClient.GetBulkCopy())
                {
                    bulkInserter.DestinationTableName = indexesReader.StorageSchema.Name;
                    bulkInserter.BatchSize = indexesReader.RecordsAffected;

                    var fields = indexesReader.StorageSchema.GetFieldsOrderedByIndex();
                    foreach (var field in fields)
                    {
                        if (field.Name == IndexStorageSchema.Fields.StringValue.Name && !(indexesReader is ValueTypeIndexesReader))
                            continue;

                        bulkInserter.AddColumnMapping(field.Name, field.Name);
                    }
                    bulkInserter.Write(indexesReader);
                }
            }
        }

        protected virtual void InsertUniques(IStructureSchema structureSchema, IStructure[] structures)
        {
            if (!structures.Any())
                return;

            var uniques = structures.SelectMany(s => s.Uniques).ToArray();
            if (!uniques.Any())
                return;

            if (uniques.Length == 1)
                MainDbClient.SingleInsertOfUniqueIndex(uniques[0], structureSchema);
            else
                BulkInsertUniques(structureSchema, uniques);
        }

        protected virtual void BulkInsertUniques(IStructureSchema structureSchema, IStructureIndex[] uniques)
        {
            if (!uniques.Any())
                return;

            var uniquesStorageSchema = new UniqueStorageSchema(structureSchema, structureSchema.GetUniquesTableName());

            using (var uniquesReader = new UniquesReader(uniquesStorageSchema, uniques))
            {
                using (var bulkInserter = MainDbClient.GetBulkCopy())
                {
                    bulkInserter.DestinationTableName = uniquesReader.StorageSchema.Name;
                    bulkInserter.BatchSize = uniques.Length;

                    foreach (var field in uniquesReader.StorageSchema.GetFieldsOrderedByIndex())
                        bulkInserter.AddColumnMapping(field.Name, field.Name);

                    bulkInserter.Write(uniquesReader);
                }
            }
        }

        protected virtual IndexInsertAction[] CreateGroupedIndexInsertActions(IStructureSchema structureSchema, IStructure[] structures)
        {
            var indexesTableNames = structureSchema.GetIndexesTableNames();
            var insertActions = new Dictionary<DataTypeCode, IndexInsertAction>(indexesTableNames.AllTableNames.Length);
            foreach (var group in structures.SelectMany(s => s.Indexes).GroupBy(i => i.DataTypeCode))
            {
                var insertAction = CreateIndexInsertActionGroup(structureSchema, indexesTableNames, group.Key, group.ToArray());
                if (insertAction.HasData)
                    insertActions.Add(group.Key, insertAction);
            }

            var mergeStringsAndEnums = insertActions.ContainsKey(DataTypeCode.String) && insertActions.ContainsKey(DataTypeCode.Enum);
            if (mergeStringsAndEnums)
            {
                var strings = insertActions[DataTypeCode.String];
                strings.Data = insertActions[DataTypeCode.Enum].Data.MergeWith(strings.Data).ToArray();
                insertActions.Remove(DataTypeCode.Enum);
            }

            return insertActions.Values.ToArray();
        }

        protected virtual IndexInsertAction CreateIndexInsertActionGroup(IStructureSchema structureSchema, IndexesTableNames indexesTableNames, DataTypeCode dataTypeCode, IStructureIndex[] indexes)
        {
            var container = new IndexInsertAction { Data = indexes };

            switch (dataTypeCode)
            {
                case DataTypeCode.IntegerNumber:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.IntegersTableName), data), dbClient);
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.IntegersTableName);
                    break;
                case DataTypeCode.FractalNumber:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.FractalsTableName), data), dbClient);
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.FractalsTableName);
                    break;
                case DataTypeCode.Bool:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.BooleansTableName), data), dbClient);
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.BooleansTableName);
                    break;
                case DataTypeCode.DateTime:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.DatesTableName), data), dbClient);
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.DatesTableName);
                    break;
                case DataTypeCode.Guid:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.GuidsTableName), data), dbClient);
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.GuidsTableName);
                    break;
                case DataTypeCode.String:
                case DataTypeCode.Enum:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => BulkInsertIndexes(new StringIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.StringsTableName), data), dbClient);
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfStringTypeIndex(data[0], indexesTableNames.StringsTableName);
                    break;
                case DataTypeCode.Unknown:
                    container.Data = container.Data.Where(i => i.DataType == TextType).ToArray();
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => BulkInsertIndexes(new TextIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.TextsTableName), data), dbClient);
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfStringTypeIndex(data[0], indexesTableNames.TextsTableName);
                    break;
            }

            return container;
        }
    }
}