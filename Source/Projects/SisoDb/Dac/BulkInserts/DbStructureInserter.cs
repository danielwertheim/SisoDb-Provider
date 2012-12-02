using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SisoDb.DbSchema;
using SisoDb.NCore.Collections;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

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

        protected const int MaxNumOfStructuresBeforeParallelEscalation = 10;
        protected readonly IDbClient MainDbClient;

        public DbStructureInserter(IDbClient mainDbClient)
        {
            MainDbClient = mainDbClient;
        }

        public virtual void Insert(IStructureSchema structureSchema, IStructure[] structures)
        {
            var groupedIndexInsertActions = new IndexInsertAction[0];

            Task task = null;
            try
            {
                task = Task.Factory.StartNew(
                    () => groupedIndexInsertActions = CreateGroupedIndexInsertActions(structureSchema, structures));

                InsertStructures(structureSchema, structures);
                InsertUniques(structureSchema, structures);

                Task.WaitAll(task);
            }
            finally
            {
                if (task != null && task.Status == TaskStatus.RanToCompletion)
                    task.Dispose();
            }

            if (!groupedIndexInsertActions.Any())
                return;

            InsertIndexes(groupedIndexInsertActions);
        }

        public virtual void InsertIndexesOnly(IStructureSchema structureSchema, IStructure[] structures)
        {
            var groupedIndexInsertActions = CreateGroupedIndexInsertActions(structureSchema, structures);
            if (!groupedIndexInsertActions.Any())
                return;

            InsertIndexes(groupedIndexInsertActions);
        }

        public virtual void Replace(IStructureSchema structureSchema, IStructure structure)
        {
            var structures = new[] { structure };
            MainDbClient.SingleUpdateOfStructure(structure, structureSchema);
            InsertUniques(structureSchema, structures);

            var groupedIndexInsertActions = CreateGroupedIndexInsertActions(structureSchema, new[] { structure });
            if (!groupedIndexInsertActions.Any())
                return;

            InsertIndexes(groupedIndexInsertActions);
        }

        protected virtual void InsertStructures(IStructureSchema structureSchema, IStructure[] structures)
        {
            if (!structures.Any())
                return;

            if (structures.Length == 1)
                MainDbClient.SingleInsertStructure(structures[0], structureSchema);
            else
                MainDbClient.BulkInsertStructures(structureSchema, structures);
        }

        protected virtual void InsertIndexes(IndexInsertAction[] groupedIndexInsertActions)
        {
            foreach (var groupedIndexInsertAction in groupedIndexInsertActions.Where(i => i.Action != null))
                groupedIndexInsertAction.Action.Invoke(groupedIndexInsertAction.Data, MainDbClient);
        }

        protected virtual ISet<SchemaField> GetStorageSchemaFieldsToSkip(bool isValueTypeIndexesReader)
        {
            var fieldsToSkip = new HashSet<SchemaField> { IndexStorageSchema.Fields.RowId };

            if (!isValueTypeIndexesReader)
                fieldsToSkip.Add(IndexStorageSchema.Fields.StringValue);
            
            return fieldsToSkip;
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
                MainDbClient.BulkInsertUniques(structureSchema, uniques);
        }

        protected virtual IndexInsertAction[] CreateGroupedIndexInsertActions(IStructureSchema structureSchema, IStructure[] structures)
        {
            var indexesTableNames = structureSchema.GetIndexesTableNames();
            var insertActions = new Dictionary<DataTypeCode, IndexInsertAction>(indexesTableNames.All.Length);
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
                var enums = insertActions[DataTypeCode.Enum];
                insertActions.Remove(DataTypeCode.Enum);

                insertActions[DataTypeCode.String] = CreateIndexInsertActionGroup(structureSchema, indexesTableNames, DataTypeCode.String, strings.Data.MergeWith(enums.Data).ToArray());
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
                        container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.IntegersTableName), data));
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.IntegersTableName);
                    break;
                case DataTypeCode.FractalNumber:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.FractalsTableName), data));
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.FractalsTableName);
                    break;
                case DataTypeCode.Bool:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.BooleansTableName), data));
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.BooleansTableName);
                    break;
                case DataTypeCode.DateTime:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.DatesTableName), data));
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.DatesTableName);
                    break;
                case DataTypeCode.Guid:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.GuidsTableName), data));
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.GuidsTableName);
                    break;
                case DataTypeCode.String:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new StringIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.StringsTableName), data));
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfStringTypeIndex(data[0], indexesTableNames.StringsTableName);
                    break;
                case DataTypeCode.Enum:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new StringIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.StringsTableName), data));
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfStringTypeIndex(data[0], indexesTableNames.StringsTableName);
                    break;
                case DataTypeCode.Text:
                    if (container.Data.Length > 1)
                        container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new TextIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.TextsTableName), data));
                    if (container.Data.Length == 1)
                        container.Action = (data, dbClient) => dbClient.SingleInsertOfStringTypeIndex(data[0], indexesTableNames.TextsTableName);
                    break;
                default:
                    container.Action = null;
                    break;
            }

            return container;
        }
    }
}