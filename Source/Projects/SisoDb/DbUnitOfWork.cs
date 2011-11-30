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

namespace SisoDb
{
    public abstract class DbUnitOfWork : DbQueryEngine, IUnitOfWork //TODO: Use composition instead.
    {
        protected DbUnitOfWork(
            ISisoDatabase db,
            IDbSchemaManager dbSchemaManager)
            : base(db, dbSchemaManager, true)
        {
        }

        public IStructureSchema GetSchema(Type type)
        {
            return Db.StructureSchemas.GetSchema(type);
        }

        public IStructureSchema GetSchema<T>() where T : class
        {
            return Db.StructureSchemas.GetSchema<T>();
        }

        public virtual void Commit()
        {
            DbClient.Flush();
        }

        protected virtual long CheckOutAndGetNextIdentity(IStructureSchema structureSchema, int numOfIds)
        {
            return DbClientNonTrans.CheckOutAndGetNextIdentity(structureSchema.Hash, numOfIds);
        }

        public virtual void Insert<T>(T item) where T : class
        {
            var structureSchema = Db.StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, CheckOutAndGetNextIdentity);

            var structure = structureBuilder.CreateStructure(item, structureSchema);

            var bulkInserter = ProviderFactory.GetDbStructureInserter(DbClient);
            bulkInserter.Insert(structureSchema, new[] { structure });
        }

        public virtual void InsertJson<T>(string json) where T : class
        {
            Insert(Db.Serializer.Deserialize<T>(json));
        }

        public virtual void InsertMany<T>(IList<T> items) where T : class
        {
            var structureSchema = Db.StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, CheckOutAndGetNextIdentity);
            
            var bulkInserter = ProviderFactory.GetDbStructureInserter(DbClient);
            bulkInserter.Insert(structureSchema, structureBuilder.CreateStructures(items, structureSchema)); //TODO: Batch?
        }

        public virtual void InsertManyJson<T>(IList<string> json) where T : class
        {
            InsertMany(Db.Serializer.DeserializeMany<T>(json).ToList());
        }

        public virtual void Update<T>(T item) where T : class
        {
            var structureSchema = Db.StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);

            var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

            var existingItem = DbClient.GetJsonById(updatedStructure.Id, structureSchema);

            if (string.IsNullOrWhiteSpace(existingItem))
                throw new SisoDbException(ExceptionMessages.UnitOfWork_NoItemExistsForUpdate.Inject(updatedStructure.Name, updatedStructure.Id.Value));

            DeleteById(structureSchema, updatedStructure.Id);

            var bulkInserter = ProviderFactory.GetDbStructureInserter(DbClient);
            bulkInserter.Insert(structureSchema, new[] { updatedStructure });
        }

        public virtual void DeleteById<T>(object id) where T : class
        {
            var structureSchema = Db.StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            DeleteById(structureSchema, StructureId.ConvertFrom(id));
        }

        private void DeleteById(IStructureSchema structureSchema, IStructureId structureId)
        {
            DbClient.DeleteById(structureId, structureSchema);
        }

        public virtual void DeleteByIds<T>(params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            var structureSchema = Db.StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            DbClient.DeleteByIds(ids.Select(StructureId.ConvertFrom), structureSchema);
        }

        public virtual void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class
        {
            var structureSchema = Db.StructureSchemas.GetSchema(TypeFor<T>.Type);

            if(!structureSchema.IdAccessor.IdType.IsIdentity())
                throw new SisoDbNotSupportedByProviderException(ProviderFactory.ProviderType, ExceptionMessages.UnitOfWork_DeleteByIdInterval_WrongIdType);

            UpsertStructureSet(structureSchema);

            DbClient.DeleteWhereIdIsBetween(StructureId.ConvertFrom(idFrom), StructureId.ConvertFrom(idTo), structureSchema);
        }

        public virtual void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
        {
            Ensure.That(expression, "expression").IsNotNull();

            var structureSchema = Db.StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var commandBuilder = ProviderFactory.CreateQueryCommandBuilder<T>(structureSchema);
            var queryCommand = commandBuilder.Where(expression).Command;
            var sql = QueryGenerator.GenerateQueryReturningStrutureIds(queryCommand);
            DbClient.DeleteByQuery(sql, structureSchema);
        }
    }
}