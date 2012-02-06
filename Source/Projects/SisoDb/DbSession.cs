using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using NCore;
using NCore.Collections;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Caching;
using SisoDb.Core;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Resources;

namespace SisoDb
{
    public abstract class DbSession : ISession, IQueryEngine, IAdvancedQueries
    {
        protected const int MaxInsertManyBatchSize = 500;
        protected const int MaxUpdateManyBatchSize = 500;

        //TODO: Why a difference
        protected const CacheConsumeModes CacheConsumeModeWhenReading = CacheConsumeModes.UpdateCacheWithDbResult;
        protected const CacheConsumeModes CacheConsumeModeWhenWriting = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

        protected readonly ISisoDbDatabase Db;
        protected readonly IDbQueryGenerator QueryGenerator;
        protected readonly ISqlStatements SqlStatements;
        protected ISisoTransaction Transaction;
        protected IDbClient TransactionalDbClient;
        protected IDbClient NonTransactionalDbClient;

        protected virtual IStructureSchemas StructureSchemas
        {
            get { return Db.StructureSchemas; }
        }

        public virtual IQueryEngine QueryEngine
        {
            get { return this; }
        }

        public virtual IAdvancedQueries Advanced
        {
            get { return this; }
        }

        protected DbSession(ISisoDbDatabase db)
        {
            Ensure.That(db, "db").IsNotNull();

            Db = db;
            SqlStatements = Db.ProviderFactory.GetSqlStatements();
            QueryGenerator = Db.ProviderFactory.GetDbQueryGenerator();

            Transaction = Db.ProviderFactory.GetRequiredTransaction();
            TransactionalDbClient = Db.ProviderFactory.GetDbClient(Db.ConnectionInfo);

            using (Db.ProviderFactory.GetSuppressedTransaction())
                NonTransactionalDbClient = Db.ProviderFactory.GetDbClient(Db.ConnectionInfo);
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);

            if (NonTransactionalDbClient != null)
            {
                NonTransactionalDbClient.Dispose();
                NonTransactionalDbClient = null;
            }

            if (TransactionalDbClient != null)
            {
                TransactionalDbClient.Dispose();
                TransactionalDbClient = null;
            }

            if (Transaction != null)
            {
                Transaction.Dispose();
                Transaction = null;
            }
        }

        //TODO: Yuck!
        protected virtual long CheckOutAndGetNextIdentity(IStructureSchema structureSchema, int numOfIds)
        {
            return NonTransactionalDbClient.CheckOutAndGetNextIdentity(structureSchema.Name, numOfIds);
        }

        public virtual IStructureSchema GetStructureSchema<T>() where T : class
        {
            return Transaction.Try(() => StructureSchemas.GetSchema<T>());
        }

        IEnumerable<T> IAdvancedQueries.NamedQuery<T>(INamedQuery query)
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                var sourceData = TransactionalDbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray());
                return Db.Serializer.DeserializeMany<T>(sourceData.ToArray());
            });
        }

        IEnumerable<TOut> IAdvancedQueries.NamedQueryAs<TContract, TOut>(INamedQuery query)
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<TContract>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                var sourceData = TransactionalDbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray());
                return Db.Serializer.DeserializeMany<TOut>(sourceData.ToArray());
            });
        }

        IEnumerable<string> IAdvancedQueries.NamedQueryAsJson<T>(INamedQuery query)
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                return TransactionalDbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray()).ToArray();
            });
        }

        IEnumerable<T> IAdvancedQueries.RawQuery<T>(IRawQuery query)
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                var sourceData = TransactionalDbClient.YieldJson(query.QueryString, query.Parameters.ToArray());
                return Db.Serializer.DeserializeMany<T>(sourceData.ToArray());
            });
        }

        IEnumerable<TOut> IAdvancedQueries.RawQueryAs<TContract, TOut>(IRawQuery query)
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<TContract>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                var sourceData = TransactionalDbClient.YieldJson(query.QueryString, query.Parameters.ToArray());
                return Db.Serializer.DeserializeMany<TOut>(sourceData.ToArray());
            });
        }

        IEnumerable<string> IAdvancedQueries.RawQueryAsJson<T>(IRawQuery query)
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                return TransactionalDbClient.YieldJson(query.QueryString, query.Parameters.ToArray()).ToArray();
            });
        }

        public virtual int Count<T>(IQuery query) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                if (!query.HasWhere)
                    return TransactionalDbClient.RowCount(structureSchema);

                var whereSql = QueryGenerator.GenerateQueryReturningStrutureIds(query);
                return TransactionalDbClient.RowCountByQuery(structureSchema, whereSql);
            });
        }

        public virtual bool Exists<T>(object id) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return TransactionalDbClient.Exists(structureId, structureSchema);

                return Db.CacheProvider.Exists<T>(
                    structureSchema,
                    structureId,
                    sid => TransactionalDbClient.Exists(sid, structureSchema));
            });
        }

        public virtual T GetById<T>(object id) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return Db.Serializer.Deserialize<T>(TransactionalDbClient.GetJsonById(structureId, structureSchema));

                return Db.CacheProvider.Consume(
                    structureSchema,
                    structureId,
                    sid => Db.Serializer.Deserialize<T>(TransactionalDbClient.GetJsonById(structureId, structureSchema)),
                    CacheConsumeModeWhenReading);
            });
        }

        public virtual IEnumerable<T> GetByIds<T>(params object[] ids) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    Db.Serializer.DeserializeMany<T>(TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).ToArray());

                return Db.CacheProvider.Consume(
                    structureSchema,
                    structureIds,
                    sids => Db.Serializer.DeserializeMany<T>(TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).ToArray()),
                    CacheConsumeModeWhenReading);
            });
        }

        public virtual IEnumerable<T> GetByIdInterval<T>(object idFrom, object idTo) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(idFrom, "idFrom").IsNotNull();
                Ensure.That(idTo, "idTo").IsNotNull();

                var structureIdFrom = StructureId.ConvertFrom(idFrom);
                var structureIdTo = StructureId.ConvertFrom(idTo);

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                if (!structureSchema.IdAccessor.IdType.IsIdentity())
                    throw new SisoDbException(
                        ExceptionMessages.SisoDbNotSupportedByProviderException.Inject(Db.ProviderFactory.ProviderType, ExceptionMessages.ReadSession_GetByIdInterval_WrongIdType));

                return Db.Serializer.DeserializeMany<T>(
                    TransactionalDbClient.GetJsonWhereIdIsBetween(structureIdFrom, structureIdTo, structureSchema).ToArray());
            });
        }

        public virtual TOut GetByIdAs<TContract, TOut>(object id)
            where TContract : class
            where TOut : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = Db.StructureSchemas.GetSchema<TContract>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return Db.Serializer.Deserialize<TOut>(TransactionalDbClient.GetJsonById(structureId, structureSchema));

                return Db.CacheProvider.Consume(
                    structureSchema,
                    structureId,
                    sid => Db.Serializer.Deserialize<TOut>(TransactionalDbClient.GetJsonById(structureId, structureSchema)),
                    CacheConsumeModeWhenReading);
            });
        }

        public virtual IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
                var structureSchema = Db.StructureSchemas.GetSchema<TContract>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return Db.Serializer.DeserializeMany<TOut>(TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).ToArray());

                return Db.CacheProvider.Consume(
                    structureSchema,
                    structureIds,
                    sids => Db.Serializer.DeserializeMany<TOut>(TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).ToArray()),
                    CacheConsumeModeWhenReading);
            });
        }

        public virtual string GetByIdAsJson<T>(object id) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                return TransactionalDbClient.GetJsonById(structureId, structureSchema);
            });
        }

        public virtual IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                return TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).ToArray();
            });
        }

        public virtual ISisoQueryable<T> Query<T>() where T : class
        {
            return Transaction.Try(() => new SisoQueryable<T>(Db.ProviderFactory.GetQueryBuilder<T>(StructureSchemas), this));
        }

        public virtual IEnumerable<T> Query<T>(IQuery query) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                IEnumerable<string> sourceData;

                if (query.IsEmpty)
                    sourceData = TransactionalDbClient.GetJsonOrderedByStructureId(structureSchema);
                else
                {
                    var sqlQuery = QueryGenerator.GenerateQuery(query);
                    sourceData = TransactionalDbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray());
                }

                return Db.Serializer.DeserializeMany<T>(sourceData.ToArray());
            });
        }

        public virtual IEnumerable<TResult> QueryAs<T, TResult>(IQuery query)
            where T : class
            where TResult : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                IEnumerable<string> sourceData;

                if (query.IsEmpty)
                    sourceData = TransactionalDbClient.GetJsonOrderedByStructureId(structureSchema);
                else
                {
                    var sqlQuery = QueryGenerator.GenerateQuery(query);
                    sourceData = TransactionalDbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray());
                }

                return Db.Serializer.DeserializeMany<TResult>(sourceData.ToArray());
            });
        }

        public virtual IEnumerable<TResult> QueryAsAnonymous<T, TResult>(IQuery query, TResult template)
            where T : class
            where TResult : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                IEnumerable<string> sourceData;

                if (query.IsEmpty)
                    sourceData = TransactionalDbClient.GetJsonOrderedByStructureId(structureSchema);
                else
                {
                    var sqlQuery = QueryGenerator.GenerateQuery(query);
                    sourceData = TransactionalDbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray());
                }

                return Db.Serializer.DeserializeManyAnonymous(sourceData.ToArray(), template);
            });
        }

        public virtual IEnumerable<string> QueryAsJson<T>(IQuery query) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                if (query.IsEmpty)
                    return TransactionalDbClient.GetJsonOrderedByStructureId(structureSchema).ToArray();

                var sqlQuery = QueryGenerator.GenerateQuery(query);

                return TransactionalDbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray()).ToArray();
            });
        }

        public virtual void Insert<T>(T item) where T : class
        {
            Transaction.Try(() =>
            {
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, Db.ProviderFactory.GetIdentityStructureIdGenerator(CheckOutAndGetNextIdentity));
                var bulkInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                bulkInserter.Insert(structureSchema, new[] { structureBuilder.CreateStructure(item, structureSchema) });
            });
        }

        public virtual void InsertJson<T>(string json) where T : class
        {
            Transaction.Try(() =>
            {
                var item = Db.Serializer.Deserialize<T>(json);
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, Db.ProviderFactory.GetIdentityStructureIdGenerator(CheckOutAndGetNextIdentity));
                var bulkInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                bulkInserter.Insert(structureSchema, new[] { structureBuilder.CreateStructure(item, structureSchema) });
            });
        }

        public virtual void InsertMany<T>(IEnumerable<T> items) where T : class
        {
            Transaction.Try(() =>
            {
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                var structureBuilder = Db.StructureBuilders.ForInserts(
                    structureSchema,
                    Db.ProviderFactory.GetIdentityStructureIdGenerator(CheckOutAndGetNextIdentity));

                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                foreach (var structuresBatch in items.Batch(MaxInsertManyBatchSize))
                    structureInserter.Insert(structureSchema, structureBuilder.CreateStructures(structuresBatch, structureSchema));
            });
        }

        public virtual void InsertManyJson<T>(IEnumerable<string> json) where T : class
        {
            Transaction.Try(() =>
            {
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                var structureBuilder = Db.StructureBuilders.ForInserts(
                    structureSchema,
                    Db.ProviderFactory.GetIdentityStructureIdGenerator(CheckOutAndGetNextIdentity));

                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                foreach (var structuresBatch in Db.Serializer.DeserializeMany<T>(json).Batch(MaxInsertManyBatchSize))
                    structureInserter.Insert(structureSchema, structureBuilder.CreateStructures(structuresBatch, structureSchema));
            });
        }

        public virtual void Update<T>(T item) where T : class
        {
            Transaction.Try(() =>
            {
                Ensure.That(item, "item").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
                var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);
                var existingItem = TransactionalDbClient.GetJsonById(updatedStructure.Id, structureSchema);

                if (string.IsNullOrWhiteSpace(existingItem))
                    throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(
                        updatedStructure.Name, updatedStructure.Id.Value));

                Db.CacheProvider.NotifyDeleting(structureSchema, updatedStructure.Id);
                TransactionalDbClient.DeleteById(updatedStructure.Id, structureSchema);

                var bulkInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                bulkInserter.Insert(structureSchema, new[] { updatedStructure });
            });
        }

        public virtual void UpdateMany<T>(Expression<Func<T, bool>> expression, Action<T> modifier) where T : class
        {
            Transaction.Try(() =>
            {
                Ensure.That(expression, "expression").IsNotNull();
                Ensure.That(modifier, "modifier").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                var deleteIds = new List<IStructureId>(MaxUpdateManyBatchSize);
                var keepQueue = new List<T>(MaxUpdateManyBatchSize);
                var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(StructureSchemas);
                var query = queryBuilder.Where(expression).Build();
                var sqlQuery = QueryGenerator.GenerateQuery(query);

                foreach (var structure in Db.Serializer.DeserializeMany<T>(
                    TransactionalDbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray())))
                {
                    var structureIdBefore = structureSchema.IdAccessor.GetValue(structure);
                    modifier.Invoke(structure);
                    var structureIdAfter = structureSchema.IdAccessor.GetValue(structure);

                    if (!structureIdBefore.Value.Equals(structureIdAfter.Value))
                        throw new SisoDbException(ExceptionMessages.WriteSession_UpdateMany_NewIdDoesNotMatchOldId.Inject(
                                structureIdAfter.Value, structureIdBefore.Value));

                    deleteIds.Add(structureIdBefore);

                    keepQueue.Add(structure);
                    if (keepQueue.Count < MaxUpdateManyBatchSize)
                        continue;

                    Db.CacheProvider.NotifyDeleting(structureSchema, deleteIds);
                    TransactionalDbClient.DeleteByIds(deleteIds, structureSchema);
                    deleteIds.Clear();

                    structureInserter.Insert(structureSchema,
                                             structureBuilder.CreateStructures(keepQueue.ToArray(), structureSchema));
                    keepQueue.Clear();
                }

                if (keepQueue.Count > 0)
                {
                    Db.CacheProvider.NotifyDeleting(structureSchema, deleteIds);
                    TransactionalDbClient.DeleteByIds(deleteIds, structureSchema);
                    deleteIds.Clear();

                    structureInserter.Insert(structureSchema,
                                             structureBuilder.CreateStructures(keepQueue.ToArray(), structureSchema));
                    keepQueue.Clear();
                }
            });
        }

        public virtual void DeleteById<T>(object id) where T : class
        {
            Transaction.Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                Db.CacheProvider.NotifyDeleting(structureSchema, structureId);
                TransactionalDbClient.DeleteById(structureId, structureSchema);
            });
        }

        public virtual void DeleteByIds<T>(params object[] ids) where T : class
        {
            Transaction.Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                Db.CacheProvider.NotifyDeleting(structureSchema, structureIds);
                TransactionalDbClient.DeleteByIds(structureIds, structureSchema);
            });
        }

        public virtual void DeleteByIdInterval<T>(object idFrom, object idTo) where T : class
        {
            Transaction.Try(() =>
            {
                Ensure.That(idFrom, "idFrom").IsNotNull();
                Ensure.That(idTo, "idTo").IsNotNull();

                var structureIdFrom = StructureId.ConvertFrom(idFrom);
                var structureIdTo = StructureId.ConvertFrom(idTo);
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                if (!structureSchema.IdAccessor.IdType.IsIdentity())
                    throw new SisoDbException(ExceptionMessages.SisoDbNotSupportedByProviderException.Inject(
                        Db.ProviderFactory.ProviderType, ExceptionMessages.WriteSession_DeleteByIdInterval_WrongIdType));

                if (Db.CacheProvider.IsEnabledFor(structureSchema))
                    throw new SisoDbException(ExceptionMessages.WriteSession_DeleteByIdIntervalAndCachingNotSupported.Inject(structureSchema.Name));

                TransactionalDbClient.DeleteWhereIdIsBetween(structureIdFrom, structureIdTo, structureSchema);
            });
        }

        public virtual void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
        {
            Transaction.Try(() =>
            {
                Ensure.That(expression, "expression").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);

                if (Db.CacheProvider.IsEnabledFor(structureSchema))
                    throw new SisoDbException(ExceptionMessages.WriteSession_DeleteByQueryAndCachingNotSupported.Inject(structureSchema.Name));

                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(StructureSchemas);
                queryBuilder.Where(expression);

                var sql = QueryGenerator.GenerateQueryReturningStrutureIds(queryBuilder.Build());
                TransactionalDbClient.DeleteByQuery(sql, structureSchema);
            });
        }
    }
}