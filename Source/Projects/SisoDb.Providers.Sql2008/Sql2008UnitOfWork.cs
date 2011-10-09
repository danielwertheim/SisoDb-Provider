using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Resources;
using SisoDb.Serialization;
using SisoDb.Structures;

namespace SisoDb.Sql2008
{
    public class Sql2008UnitOfWork : Sql2008QueryEngine, IUnitOfWork
    {
        private const int BatchSize = 1000;

        protected IStructureBuilder StructureBuilder { get; private set; }
        protected IdentityStructureIdGenerator IdentityStructureIdGenerator { get; private set; }

        protected internal Sql2008UnitOfWork(
            ISisoProviderFactory providerFactory,
            ISisoConnectionInfo connectionInfo,
            IDbSchemaManager dbSchemaManager,
            IStructureSchemas structureSchemas,
            IJsonSerializer jsonSerializer,
            IStructureBuilder structureBuilder)
            : base(providerFactory, connectionInfo, dbSchemaManager, structureSchemas, jsonSerializer)
        {
            Ensure.That(() => structureBuilder).IsNotNull();

            StructureBuilder = structureBuilder;
            IdentityStructureIdGenerator = ProviderFactory.GetIdentityStructureIdGenerator(DbClientNonTrans);
        }

        public void Commit()
        {
            DbClientNonTrans.Flush();
        }

        public void Insert<T>(T item) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var structure = StructureBuilder.CreateStructure(item, structureSchema, CreateStructureBuilderOptions(structureSchema.IdAccessor.IdType));

            var bulkInserter = ProviderFactory.GetDbBulkInserter(DbClientNonTrans);
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

            var structureBuilderOptions = CreateStructureBuilderOptions(structureSchema.IdAccessor.IdType);
            
            var bulkInserter = ProviderFactory.GetDbBulkInserter(DbClientNonTrans);

            foreach (var batchOfStructures in StructureBuilder.CreateStructureBatches(items, structureSchema, BatchSize, structureBuilderOptions))
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

            var structureBuilderOptions = CreateStructureBuilderOptions(structureSchema.IdAccessor.IdType);
            structureBuilderOptions.KeepStructureId = true;

            var updatedStructure = StructureBuilder.CreateStructure(item, structureSchema, structureBuilderOptions);

            var existingItem = GetByIdAsJson<T>(updatedStructure.Id.Value);

            if (string.IsNullOrWhiteSpace(existingItem))
                throw new SisoDbException(ExceptionMessages.UnitOfWork_NoItemExistsForUpdate.Inject(updatedStructure.Name, updatedStructure.Id));

            DeleteById(structureSchema, updatedStructure.Id.Value);

            var bulkInserter = ProviderFactory.GetDbBulkInserter(DbClientNonTrans);
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
            DbClientNonTrans.DeleteById(structureId, structureSchema);
        }

        public void DeleteByIds<T>(IEnumerable<ValueType> ids) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            DbClientNonTrans.DeleteByIds(ids, structureSchema.IdAccessor.IdType, structureSchema);
        }

        public void DeleteByIdInterval<T>(ValueType idFrom, ValueType idTo) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            DbClientNonTrans.DeleteWhereIdIsBetween(idFrom, idTo, structureSchema);
        }

        public void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
        {
            Ensure.That(() => expression).IsNotNull();

            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var commandBuilder = CommandBuilderFactory.CreateQueryCommandBuilder<T>();
            var queryCommand = commandBuilder.Where(expression).Command;
            var sql = QueryGenerator.GenerateWhere(queryCommand);
            DbClientNonTrans.DeleteByQuery(sql, structureSchema.IdAccessor.DataType, structureSchema);
        }

        protected StructureBuilderOptions CreateStructureBuilderOptions(StructureIdTypes structureIdType)
        {
            return StructureBuilderOptionsFactory.Create(structureIdType, IdentityStructureIdGenerator);
        }
    }
}