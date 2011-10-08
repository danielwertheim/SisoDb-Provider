using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Serialization;

namespace SisoDb.Sql2008
{
    public class Sql2008UnitOfWork : Sql2008QueryEngine, IUnitOfWork
    {
        private const int BatchSize = 1000;

        protected readonly IStructureBuilder StructureBuilder;

        protected internal Sql2008UnitOfWork(
            ISisoProviderFactory providerFactory,
            IDbClient dbClient,
            IDbSchemaManager dbSchemaManager,
            IDbSchemaUpserter dbSchemaUpserter,
            IStructureSchemas structureSchemas,
            IStructureBuilder structureBuilder,
            IJsonSerializer jsonSerializer,
            IDbQueryGenerator queryGenerator,
            ICommandBuilderFactory commandBuilderFactory)
            : base(providerFactory, dbClient, dbSchemaManager, dbSchemaUpserter, structureSchemas, jsonSerializer, queryGenerator, commandBuilderFactory)
        {
            Ensure.That(() => structureBuilder).IsNotNull();

            StructureBuilder = structureBuilder;
        }

        public void Commit()
        {
            DbClient.Flush();
        }

        public void Insert<T>(T item) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var structure = StructureBuilder.CreateStructure(item, structureSchema);

            var bulkInserter = ProviderFactory.GetDbBulkInserter(DbClient);
            bulkInserter.Insert(structureSchema, new[] { structure });
        }

        public void InsertJson<T>(string json) where T : class
        {
            Insert(JsonSerializer.Deserialize<T>(json));
        }

        public void InsertMany<T>(IList<T> items) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var bulkInserter = ProviderFactory.GetDbBulkInserter(DbClient);

            foreach (var batchOfStructures in StructureBuilder.CreateStructureBatches(items, structureSchema, BatchSize))
                bulkInserter.Insert(structureSchema, batchOfStructures);
        }

        public void InsertManyJson<T>(IList<string> json) where T : class
        {
            InsertMany(JsonSerializer.DeserializeMany<T>(json).ToList());
        }

        public void Update<T>(T item) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var updatedStructure = StructureBuilder.CreateStructure(item, structureSchema);

            var existingItem = GetByIdAsJson<T>(updatedStructure.Id.Value);

            if (string.IsNullOrWhiteSpace(existingItem))
                throw new SisoDbException(ExceptionMessages.UnitOfWork_NoItemExistsForUpdate.Inject(updatedStructure.Name, updatedStructure.Id));

            DeleteById(structureSchema, updatedStructure.Id.Value);

            var bulkInserter = ProviderFactory.GetDbBulkInserter(DbClient);
            bulkInserter.Insert(structureSchema, new[] { updatedStructure });
        }

        public void DeleteById<T>(ValueType id) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            DeleteById(structureSchema, id);
        }

        private void DeleteById(IStructureSchema structureSchema, ValueType structureId)
        {
            DbClient.DeleteById(structureId, structureSchema);
        }

        public void DeleteByIds<T>(IEnumerable<ValueType> ids) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            DbClient.DeleteByIds(ids, structureSchema.IdAccessor.IdType, structureSchema);
        }

        public void DeleteByIdInterval<T>(ValueType idFrom, ValueType idTo) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            DbClient.DeleteWhereIdIsBetween(idFrom, idTo, structureSchema);
        }

        public void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
        {
            Ensure.That(() => expression).IsNotNull();

            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var commandBuilder = CommandBuilderFactory.CreateQueryCommandBuilder<T>();
            var queryCommand = commandBuilder.Where(expression).Command;
            var sql = QueryGenerator.GenerateWhere(queryCommand);
            DbClient.DeleteByQuery(sql, structureSchema.IdAccessor.DataType, structureSchema);
        }
    }
}