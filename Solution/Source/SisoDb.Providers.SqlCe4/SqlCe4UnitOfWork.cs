using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.Core;
using SisoDb.Providers.Dac;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlCe4.BulkInserts;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Serialization;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlCe4
{
    public class SqlCe4UnitOfWork : SqlCe4QueryEngine, IUnitOfWork
    {
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IStructureBuilder _structureBuilder;

        protected internal SqlCe4UnitOfWork(
            ISqlDbClient dbClient,
            IIdentityGenerator identityGenerator,
            IDbSchemaManager dbSchemaManager,
            IDbSchemaUpserter dbSchemaUpserter,
            IStructureSchemas structureSchemas,
            IStructureBuilder structureBuilder,
            IJsonSerializer jsonSerializer,
            IJsonBatchDeserializer jsonBatchDeserializer,
            ISqlQueryGenerator queryGenerator,
            ICommandBuilderFactory commandBuilderFactory)
            : base(dbClient, dbSchemaManager, dbSchemaUpserter, structureSchemas, jsonSerializer, jsonBatchDeserializer, queryGenerator, commandBuilderFactory)
        {
            _identityGenerator = identityGenerator.AssertNotNull("identityGenerator");
            _structureBuilder = structureBuilder.AssertNotNull("structureBuilder");
        }

        public void Commit()
        {
            DbClient.Flush();
        }

        [DebuggerStepThrough]
        public void Insert<T>(T item) where T : class
        {
            InsertMany(new[] { item });
        }

        public void InsertJson<T>(string json) where T : class
        {
            Insert(JsonSerializer.ToItemOrNull<T>(json));
        }

        public void InsertMany<T>(IList<T> items) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            DoInsert(structureSchema, items);
        }

        public void InsertManyJson<T>(IList<string> json) where T : class
        {
            InsertMany(JsonBatchDeserializer.Deserialize<T>(json).ToList());
        }

        private void DoInsert<T>(IStructureSchema structureSchema, IEnumerable<T> items) where T : class
        {
            if (items.Count() < 1)
                return;

            var hasIdentity = structureSchema.IdAccessor.IdType == IdTypes.Identity;

            var bulkInserter = new SqlBulkInserter(DbClient);

            if (hasIdentity)
            {
                var seed = _identityGenerator.CheckOutAndGetSeed(structureSchema, items.Count());

                foreach (var structures in _structureBuilder.CreateBatchedIdentityStructures(items, structureSchema, 1000, seed))
                    bulkInserter.Insert(structureSchema, structures);
            }
            else
            {
                foreach (var structures in _structureBuilder.CreateBatchedGuidStructures(items, structureSchema, 1000))
                    bulkInserter.Insert(structureSchema, structures);
            }
        }

        public void Update<T>(T item) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            var updatedStructure = _structureBuilder.CreateStructure(item, structureSchema);

            var existingItem = GetByIdAsJson<T>((Guid)updatedStructure.Id.Value);
            if (string.IsNullOrWhiteSpace(existingItem))
                throw new SisoDbException(
                    ExceptionMessages.UnitOfWork_NoItemExistsForUpdate.Inject(updatedStructure.Name, updatedStructure.Id));

            DeleteById<T>(updatedStructure.Id);

            var bulkInserter = new SqlBulkInserter(DbClient);
            bulkInserter.Insert(structureSchema, new[] { updatedStructure });
        }

        [DebuggerStepThrough]
        public void DeleteById<T>(Guid sisoId) where T : class
        {
            DeleteById<T>(SisoId.NewGuidId(sisoId));
        }

        [DebuggerStepThrough]
        public void DeleteById<T>(int sisoId) where T : class
        {
            DeleteById<T>(SisoId.NewIdentityId(sisoId));
        }

        private void DeleteById<T>(ISisoId sisoId) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            DbClient.DeleteById(
                sisoId.Value,
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());
        }

        [DebuggerStepThrough]
        public void DeleteByIds<T>(IEnumerable<int> ids) where T : class
        {
            DeleteByIds<T>(ids.Select(id => (ValueType)id), IdTypes.Identity);
        }

        [DebuggerStepThrough]
        public void DeleteByIds<T>(IEnumerable<Guid> ids) where T : class
        {
            DeleteByIds<T>(ids.Select(id => (ValueType)id), IdTypes.Guid);
        }

        [DebuggerStepThrough]
        public void DeleteByIdInterval<T>(int idFrom, int idTo) where T : class
        {
            DeleteWhereIdIsBetween<T>(idFrom, idTo);
        }

        [DebuggerStepThrough]
        public void DeleteByIdInterval<T>(Guid idFrom, Guid idTo) where T : class
        {
            DeleteWhereIdIsBetween<T>(idFrom, idTo);
        }

        private void DeleteWhereIdIsBetween<T>(ValueType idFrom, ValueType idTo) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            DbClient.DeleteWhereIdIsBetween(
                idFrom, idTo,
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());
        }

        private void DeleteByIds<T>(IEnumerable<ValueType> ids, IdTypes idType) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            DbClient.DeleteByIds(
                ids,
                idType,
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());
        }

        public void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
        {
            expression.AssertNotNull("expression");

            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            var commandBuilder = CommandBuilderFactory.CreateQueryCommandBuilder<T>();
            var queryCommand = commandBuilder.Where(expression).Command;
            var sql = QueryGenerator.GenerateWhere(queryCommand);
            DbClient.DeleteByQuery(sql,
                structureSchema.IdAccessor.DataType,
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());
        }

        private void UpsertStructureSet(IStructureSchema structureSchema)
        {
            DbSchemaManager.UpsertStructureSet(structureSchema, DbSchemaUpserter);
        }
    }
}