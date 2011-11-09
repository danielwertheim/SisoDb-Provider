using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Resources;
using SisoDb.Serialization;
using SisoDb.Structures;

namespace SisoDb
{
    public abstract class DbUnitOfWork : DbQueryEngine, IUnitOfWork
    {
        private const int BatchSize = 1000;

        protected IStructureBuilder StructureBuilder { get; private set; }
        protected IdentityStructureIdGenerator IdentityStructureIdGenerator { get; private set; }

        protected DbUnitOfWork(
            ISisoConnectionInfo connectionInfo,
            IDbSchemaManager dbSchemaManager,
            IStructureSchemas structureSchemas,
            IJsonSerializer jsonSerializer,
            IStructureBuilder structureBuilder)
            : base(connectionInfo, true, dbSchemaManager, structureSchemas, jsonSerializer)
        {
            Ensure.That(structureBuilder, "structureBuilder").IsNotNull();

            StructureBuilder = structureBuilder;
            IdentityStructureIdGenerator = ProviderFactory.GetIdentityStructureIdGenerator(DbClientNonTrans);
        }

        public virtual void Commit()
        {
            DbClient.Flush();
        }

        protected StructureBuilderOptions CreateStructureBuilderOptions(StructureIdTypes structureIdType)
        {
            return StructureBuilderOptionsFactory.Create(structureIdType, IdentityStructureIdGenerator);
        }

        public virtual void Insert<T>(T item) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var structure = StructureBuilder.CreateStructure(item, structureSchema, CreateStructureBuilderOptions(structureSchema.IdAccessor.IdType));

            var bulkInserter = ProviderFactory.GetDbBulkInserter(DbClient);
            bulkInserter.Insert(structureSchema, new[] { structure });
        }

        public virtual void InsertJson<T>(string json) where T : class
        {
            Insert(JsonSerializer.Deserialize<T>(json));
        }

        public virtual void InsertMany<T>(IList<T> items) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var structureBuilderOptions = CreateStructureBuilderOptions(structureSchema.IdAccessor.IdType);
            
            var bulkInserter = ProviderFactory.GetDbBulkInserter(DbClient);

            foreach (var batchOfStructures in StructureBuilder.CreateStructureBatches(items, structureSchema, BatchSize, structureBuilderOptions))
                bulkInserter.Insert(structureSchema, batchOfStructures);
        }

        public virtual void InsertManyJson<T>(IList<string> json) where T : class
        {
            InsertMany(JsonSerializer.DeserializeMany<T>(json).ToList());
        }

        public virtual void Update<T>(T item) where T : class
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

            var bulkInserter = ProviderFactory.GetDbBulkInserter(DbClient);
            bulkInserter.Insert(structureSchema, new[] { updatedStructure });
        }

        public virtual void DeleteById<T>(ValueType id) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            DeleteById(structureSchema, id);
        }

        private void DeleteById(IStructureSchema structureSchema, ValueType structureId)
        {
            DbClient.DeleteById(structureId, structureSchema);
        }

        public virtual void DeleteByIds<T>(params ValueType[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            DbClient.DeleteByIds(ids, structureSchema.IdAccessor.IdType, structureSchema);
        }

        public virtual void DeleteByIdInterval<T>(ValueType idFrom, ValueType idTo) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            if(!structureSchema.IdAccessor.IdType.IsIdentity())
                throw new SisoDbNotSupportedByProviderException(ProviderType, ExceptionMessages.UnitOfWork_DeleteByIdInterval_WrongIdType);

            UpsertStructureSet(structureSchema);

            DbClient.DeleteWhereIdIsBetween(idFrom, idTo, structureSchema);
        }

        public virtual void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
        {
            Ensure.That(expression, "expression").IsNotNull();

            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var commandBuilder = ProviderFactory.CreateQueryCommandBuilder<T>(structureSchema);
            var queryCommand = commandBuilder.Where(expression).Command;
            var sql = QueryGenerator.GenerateQueryReturningStrutureIds(queryCommand);
            DbClient.DeleteByQuery(sql, structureSchema.IdAccessor.DataType, structureSchema);
        }
    }
}