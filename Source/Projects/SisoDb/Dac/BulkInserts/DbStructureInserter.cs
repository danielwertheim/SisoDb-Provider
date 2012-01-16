using System;
using System.Collections.Generic;
using System.Linq;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Structures;

namespace SisoDb.Dac.BulkInserts
{
    public class DbStructureInserter : IStructureInserter
    {
    	private static readonly Type TextType;

        private const int MaxStructureBatchSize = 1000;
        private const int MaxIndexesBatchSize = 6000;
        private const int MaxUniquesBatchSize = 6000;

        private readonly IDbClient _dbClient;

    	static DbStructureInserter()
    	{
    		TextType = typeof (Text);
    	}

        public DbStructureInserter(IDbClient dbClient)
        {
            _dbClient = dbClient;
        }

        public virtual void Insert(IStructureSchema structureSchema, IStructure[] structures)
        {
            if (structures.Length == 1)
                SingleInsertStructure(structureSchema, structures[0]);
            else
                BulkInsertStructures(structureSchema, structures);

			InsertIndexes(structureSchema, structures);

			BulkInsertUniques(structureSchema, structures);
        }

        protected virtual void SingleInsertStructure(IStructureSchema structureSchema, IStructure structure)
        {
            var sql = "insert into [{0}] ([{1}], [{2}]) values (@{1}, @{2});".Inject(
                structureSchema.GetStructureTableName(),
                StructureStorageSchema.Fields.Id.Name,
                StructureStorageSchema.Fields.Json.Name);

            _dbClient.ExecuteNonQuery(sql,
                new DacParameter(StructureStorageSchema.Fields.Id.Name, structure.Id.Value),
                new DacParameter(StructureStorageSchema.Fields.Json.Name, structure.Data));
        }

		protected virtual void SingleInsertIntoValueTypeIndexesOfX(string valueTypeIndexesTableName, IStructureIndex structureIndex)
		{
			var sql = "insert into [{0}] ([{1}], [{2}], [{3}], [{4}]) values (@{1}, @{2}, @{3}, @{4})".Inject(
				valueTypeIndexesTableName,
				IndexStorageSchema.Fields.StructureId.Name,
				IndexStorageSchema.Fields.MemberPath.Name,
				IndexStorageSchema.Fields.Value.Name,
				IndexStorageSchema.Fields.StringValue.Name);

			_dbClient.ExecuteNonQuery(sql,
				new DacParameter(IndexStorageSchema.Fields.StructureId.Name, structureIndex.StructureId.Value),
				new DacParameter(IndexStorageSchema.Fields.MemberPath.Name, structureIndex.Path),
				new DacParameter(IndexStorageSchema.Fields.Value.Name, structureIndex.Value),
				new DacParameter(IndexStorageSchema.Fields.StringValue.Name, SisoEnvironment.StringConverter.AsString(structureIndex.Value)));
		}

		protected virtual void SingleInsertIntoStringishIndexesOfX(string stringishIndexesTableName, IStructureIndex structureIndex)
		{
			var sql = "insert into [{0}] ([{1}], [{2}], [{3}]) values (@{1}, @{2}, @{3})".Inject(
				stringishIndexesTableName,
				IndexStorageSchema.Fields.StructureId.Name,
				IndexStorageSchema.Fields.MemberPath.Name,
				IndexStorageSchema.Fields.Value.Name);

			_dbClient.ExecuteNonQuery(sql,
				new DacParameter(IndexStorageSchema.Fields.StructureId.Name, structureIndex.StructureId.Value),
				new DacParameter(IndexStorageSchema.Fields.MemberPath.Name, structureIndex.Path),
				new DacParameter(IndexStorageSchema.Fields.Value.Name, structureIndex.Value.ToString()));
		}

        protected virtual void BulkInsertStructures(IStructureSchema structureSchema, IEnumerable<IStructure> structures)
        {
            var structureStorageSchema = new StructureStorageSchema(structureSchema, structureSchema.GetStructureTableName());

            using (var structuresReader = new StructuresReader(structureStorageSchema, structures))
            {
                using (var bulkInserter = _dbClient.GetBulkCopy())
                {
                    bulkInserter.BatchSize = structuresReader.RecordsAffected > MaxStructureBatchSize ? MaxStructureBatchSize : structuresReader.RecordsAffected;
                    bulkInserter.DestinationTableName = structuresReader.StorageSchema.Name;

                    foreach (var field in structuresReader.StorageSchema.GetFieldsOrderedByIndex())
                        bulkInserter.AddColumnMapping(field.Name, field.Name);

                    bulkInserter.Write(structuresReader);
                }
            }
        }

        protected virtual void InsertIndexes(IStructureSchema structureSchema, IEnumerable<IStructure> structures)
        {
			var indexesTableNames = structureSchema.GetIndexesTableNames();
        	var structureIndexes = structures.SelectMany(s => s.Indexes).ToArray();
			
			var integerIndexes = structureIndexes.Where(i => i.DataTypeCode == DataTypeCode.IntegerNumber).ToArray();
			if(integerIndexes.Length > 1)
				BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.IntegersTableName), integerIndexes));
			else if(integerIndexes.Length == 1)
				SingleInsertIntoValueTypeIndexesOfX(indexesTableNames.IntegersTableName, integerIndexes[0]);
			
			var fractalIndexes = structureIndexes.Where(i => i.DataTypeCode == DataTypeCode.FractalNumber).ToArray();
			if(fractalIndexes.Length > 1)
        		BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.FractalsTableName), fractalIndexes));
			else if (fractalIndexes.Length == 1)
				SingleInsertIntoValueTypeIndexesOfX(indexesTableNames.FractalsTableName, fractalIndexes[0]);

			var boolIndexes = structureIndexes.Where(i => i.DataTypeCode == DataTypeCode.Bool).ToArray();
			if(boolIndexes.Length > 1)
				BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.BooleansTableName), boolIndexes));
			else if (boolIndexes.Length == 1)
				SingleInsertIntoValueTypeIndexesOfX(indexesTableNames.BooleansTableName, boolIndexes[0]);

			var dateIndexes = structureIndexes.Where(i => i.DataTypeCode == DataTypeCode.DateTime).ToArray();
			if(dateIndexes.Length > 1)
				BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.DatesTableName), dateIndexes));
			else if (dateIndexes.Length == 1)
				SingleInsertIntoValueTypeIndexesOfX(indexesTableNames.DatesTableName, dateIndexes[0]);

			var guidIndexes = structureIndexes.Where(i => i.DataTypeCode == DataTypeCode.Guid).ToArray();
			if(guidIndexes.Length > 1)
				BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.GuidsTableName), guidIndexes));
			else if (guidIndexes.Length == 1)
				SingleInsertIntoValueTypeIndexesOfX(indexesTableNames.GuidsTableName, guidIndexes[0]);

			var stringIndexes = structureIndexes.Where(i => i.DataTypeCode == DataTypeCode.String || i.DataTypeCode == DataTypeCode.Enum).ToArray();
			if(stringIndexes.Length > 1)
				BulkInsertIndexes(new StringIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.StringsTableName), stringIndexes));
			else if (stringIndexes.Length == 1)
				SingleInsertIntoStringishIndexesOfX(indexesTableNames.StringsTableName, stringIndexes[0]);

			var textIndexes = structureIndexes.Where(i => i.DataType == TextType).ToArray();
			if(textIndexes.Length > 1)
				BulkInsertIndexes(new TextIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.TextsTableName), textIndexes));
			else if (textIndexes.Length == 1)
				SingleInsertIntoStringishIndexesOfX(indexesTableNames.TextsTableName, textIndexes[0]);
        }

		protected virtual void BulkInsertIndexes(IndexesReader indexesReader)
		{
			using (indexesReader)
			{
				using (var bulkInserter = _dbClient.GetBulkCopy())
				{
					bulkInserter.BatchSize = indexesReader.RecordsAffected > MaxIndexesBatchSize
						? MaxIndexesBatchSize
						: indexesReader.RecordsAffected;
					bulkInserter.DestinationTableName = indexesReader.StorageSchema.Name;

					var fields = indexesReader.StorageSchema.GetFieldsOrderedByIndex();
					foreach (var field in fields)
					{
						if(field.Name == IndexStorageSchema.Fields.StringValue.Name && !(indexesReader is ValueTypeIndexesReader))
							continue;

						bulkInserter.AddColumnMapping(field.Name, field.Name);
					}
					bulkInserter.Write(indexesReader);
				}
			}
		}

        protected virtual void BulkInsertUniques(IStructureSchema structureSchema, IEnumerable<IStructure> structures)
        {
            var uniques = structures.SelectMany(s => s.Uniques).ToArray();
            if (uniques.Length <= 0)
                return;

            var uniquesStorageSchema = new UniqueStorageSchema(structureSchema, structureSchema.GetUniquesTableName());

            using (var uniquesReader = new UniquesReader(uniquesStorageSchema, uniques))
            {
                using (var bulkInserter = _dbClient.GetBulkCopy())
                {
                    bulkInserter.BatchSize = uniquesReader.RecordsAffected > MaxUniquesBatchSize ? MaxUniquesBatchSize : uniquesReader.RecordsAffected;
                    bulkInserter.DestinationTableName = uniquesReader.StorageSchema.Name;

                    foreach (var field in uniquesReader.StorageSchema.GetFieldsOrderedByIndex())
                        bulkInserter.AddColumnMapping(field.Name, field.Name);

                    bulkInserter.Write(uniquesReader);
                }
            }
        }
    }
}