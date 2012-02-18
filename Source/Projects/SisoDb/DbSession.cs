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
    public abstract class DbSession : ISession, IQueryEngine, IAdvanced
    {
        protected const int MaxInsertManyBatchSize = 500;
        protected const int MaxUpdateManyBatchSize = 500;

        protected readonly ISisoDbDatabase Db;
        protected readonly IDbQueryGenerator QueryGenerator;
        protected readonly ISqlStatements SqlStatements;
        protected ITransactionalDbClient TransactionalDbClient;
        protected IDbClient NonTransactionalDbClient;

        protected virtual IStructureSchemas StructureSchemas
        {
            get { return Db.StructureSchemas; }
        }

        public virtual IQueryEngine QueryEngine
        {
            get { return this; }
        }

        public virtual IAdvanced Advanced
        {
            get { return this; }
        }

        protected virtual CacheConsumeModes CacheConsumeMode { get; private set; }

        protected DbSession(ISisoDbDatabase db)
        {
            Ensure.That(db, "db").IsNotNull();

            Db = db;
            SqlStatements = Db.ProviderFactory.GetSqlStatements();
            QueryGenerator = Db.ProviderFactory.GetDbQueryGenerator();

            NonTransactionalDbClient = Db.ProviderFactory.GetNonTransactionalDbClient(Db.ConnectionInfo);
            TransactionalDbClient = Db.ProviderFactory.GetTransactionalDbClient(Db.ConnectionInfo);

            CacheConsumeMode = CacheConsumeModes.UpdateCacheWithDbResult;
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);

            if (TransactionalDbClient != null)
            {
                TransactionalDbClient.Dispose();
                TransactionalDbClient = null;
            }
            if (NonTransactionalDbClient != null)
            {
                NonTransactionalDbClient.Dispose();
                NonTransactionalDbClient = null;
            }
        }

        protected virtual void Try(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch
            {
                TransactionalDbClient.MarkAsFailed();
                throw;
            }
        }

        protected virtual T Try<T>(Func<T> action)
        {
            try
            {
                return action.Invoke();
            }
            catch
            {
                TransactionalDbClient.MarkAsFailed();
                throw;
            }
        }

        protected virtual long CheckOutAndGetNextIdentity(IStructureSchema structureSchema, int numOfIds)
        {
            return NonTransactionalDbClient.CheckOutAndGetNextIdentity(structureSchema.Name, numOfIds);
        }

        protected virtual IStructureSchema UpsertStructureSchema<T>() where T : class
        {
            var structureSchema = Db.StructureSchemas.GetSchema<T>();
            Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);
            return structureSchema;
        }

        public virtual IStructureSchema GetStructureSchema<T>() where T : class
        {
            return Try(() => StructureSchemas.GetSchema<T>());
        }

        void IAdvanced.DeleteByQuery<T>(Expression<Func<T, bool>> expression)
        {
            Try(() =>
            {
                Ensure.That(expression, "expression").IsNotNull();

                var structureSchema = UpsertStructureSchema<T>();
                Db.CacheProvider.NotifyOfPurge(structureSchema);

                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(StructureSchemas);
                queryBuilder.Where(expression);

                var sql = QueryGenerator.GenerateQueryReturningStrutureIds(queryBuilder.Build());
                TransactionalDbClient.DeleteByQuery(sql, structureSchema);
            });
        }

        IEnumerable<T> IAdvanced.NamedQuery<T>(INamedQuery query)
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                UpsertStructureSchema<T>();

                var sourceData = TransactionalDbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray());

                return Db.Serializer.DeserializeMany<T>(sourceData.ToArray());
            });
        }

        IEnumerable<TOut> IAdvanced.NamedQueryAs<TContract, TOut>(INamedQuery query)
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                UpsertStructureSchema<TContract>();

                var sourceData = TransactionalDbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray());

                return Db.Serializer.DeserializeMany<TOut>(sourceData.ToArray());
            });
        }

        IEnumerable<string> IAdvanced.NamedQueryAsJson<T>(INamedQuery query)
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                UpsertStructureSchema<T>();

                return TransactionalDbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray()).ToArray();
            });
        }

        IEnumerable<T> IAdvanced.RawQuery<T>(IRawQuery query)
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                UpsertStructureSchema<T>();

                var sourceData = TransactionalDbClient.YieldJson(query.QueryString, query.Parameters.ToArray());
                return Db.Serializer.DeserializeMany<T>(sourceData.ToArray());
            });
        }

        IEnumerable<TOut> IAdvanced.RawQueryAs<TContract, TOut>(IRawQuery query)
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                UpsertStructureSchema<TContract>();

                var sourceData = TransactionalDbClient.YieldJson(query.QueryString, query.Parameters.ToArray());
                return Db.Serializer.DeserializeMany<TOut>(sourceData.ToArray());
            });
        }

        IEnumerable<string> IAdvanced.RawQueryAsJson<T>(IRawQuery query)
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                UpsertStructureSchema<T>();

                return TransactionalDbClient.YieldJson(query.QueryString, query.Parameters.ToArray()).ToArray();
            });
        }

        void IAdvanced.UpdateMany<T>(Expression<Func<T, bool>> expression, Action<T> modifier)
        {
            Try(() =>
            {
                Ensure.That(expression, "expression").IsNotNull();
                Ensure.That(modifier, "modifier").IsNotNull();

                var structureSchema = UpsertStructureSchema<T>();

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

        public virtual int Count<T>(IQuery query) where T : class
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = UpsertStructureSchema<T>();

                if (!query.HasWhere)
                    return TransactionalDbClient.RowCount(structureSchema);

                var whereSql = QueryGenerator.GenerateQueryReturningStrutureIds(query);
                return TransactionalDbClient.RowCountByQuery(structureSchema, whereSql);
            });
        }

        public virtual bool Exists<T>(object id) where T : class
        {
            return Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = UpsertStructureSchema<T>();

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return TransactionalDbClient.Exists(structureId, structureSchema);

                return Db.CacheProvider.Exists(
                    structureSchema,
                    structureId,
                    sid => TransactionalDbClient.Exists(sid, structureSchema));
            });
        }

        public virtual T GetById<T>(object id) where T : class
        {
            return Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = UpsertStructureSchema<T>();

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return Db.Serializer.Deserialize<T>(TransactionalDbClient.GetJsonById(structureId, structureSchema));

                return Db.CacheProvider.Consume(
                    structureSchema,
                    structureId,
                    sid => Db.Serializer.Deserialize<T>(TransactionalDbClient.GetJsonById(structureId, structureSchema)),
                    CacheConsumeMode);
            });
        }

        public virtual IEnumerable<T> GetByIds<T>(params object[] ids) where T : class
        {
            return Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
                var structureSchema = UpsertStructureSchema<T>();

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    Db.Serializer.DeserializeMany<T>(TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).ToArray());

                return Db.CacheProvider.Consume(
                    structureSchema,
                    structureIds,
                    sids => Db.Serializer.DeserializeMany<T>(TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).ToArray()),
                    CacheConsumeMode);
            });
        }

        public virtual TOut GetByIdAs<TContract, TOut>(object id)
            where TContract : class
            where TOut : class
        {
            return Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = UpsertStructureSchema<TContract>();

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return Db.Serializer.Deserialize<TOut>(TransactionalDbClient.GetJsonById(structureId, structureSchema));

                return Db.CacheProvider.Consume(
                    structureSchema,
                    structureId,
                    sid => Db.Serializer.Deserialize<TOut>(TransactionalDbClient.GetJsonById(structureId, structureSchema)),
                    CacheConsumeMode);
            });
        }

        public virtual IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class
        {
            return Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
                var structureSchema = UpsertStructureSchema<TContract>();

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return Db.Serializer.DeserializeMany<TOut>(TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).ToArray());

                return Db.CacheProvider.Consume(
                    structureSchema,
                    structureIds,
                    sids => Db.Serializer.DeserializeMany<TOut>(TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).ToArray()),
                    CacheConsumeMode);
            });
        }

        public virtual string GetByIdAsJson<T>(object id) where T : class
        {
            return Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = UpsertStructureSchema<T>();

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return TransactionalDbClient.GetJsonById(structureId, structureSchema);

                var item = Db.CacheProvider.Consume(
                    structureSchema,
                    structureId,
                    sid => Db.Serializer.Deserialize<T>(TransactionalDbClient.GetJsonById(structureId, structureSchema)),
                    CacheConsumeMode);

                return Db.Serializer.Serialize(item);
            });
        }

        public virtual IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class
        {
            return Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
                var structureSchema = UpsertStructureSchema<T>();

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).ToArray();

                var items = Db.CacheProvider.Consume(
                    structureSchema,
                    structureIds,
                    sid => Db.Serializer.DeserializeMany<T>(TransactionalDbClient.GetJsonByIds(structureIds, structureSchema)).ToArray(),
                    CacheConsumeMode);

                return Db.Serializer.SerializeMany(items).ToArray();
            });
        }

        public virtual ISisoQueryable<T> Query<T>() where T : class
        {
            return Try(() => new SisoQueryable<T>(Db.ProviderFactory.GetQueryBuilder<T>(StructureSchemas), this));
        }

        public virtual IEnumerable<T> Query<T>(IQuery query) where T : class
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = UpsertStructureSchema<T>();

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
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = UpsertStructureSchema<T>();

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
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = UpsertStructureSchema<T>();

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
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = UpsertStructureSchema<T>();

                if (query.IsEmpty)
                    return TransactionalDbClient.GetJsonOrderedByStructureId(structureSchema).ToArray();

                var sqlQuery = QueryGenerator.GenerateQuery(query);

                return TransactionalDbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray()).ToArray();
            });
        }

        public virtual void Insert<T>(T item) where T : class
        {
            Try(() =>
            {
                Ensure.That(item, "item").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = UpsertStructureSchema<T>();

                var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, Db.ProviderFactory.GetIdentityStructureIdGenerator(CheckOutAndGetNextIdentity));
                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                structureInserter.Insert(structureSchema, new[] { structureBuilder.CreateStructure(item, structureSchema) });
            });
        }

        public void InsertAs<T>(object item) where T : class
        {
            Try(() =>
            {
                Ensure.That(item, "item").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var json = Db.Serializer.Serialize(item);
                var realItem = Db.Serializer.Deserialize<T>(json);

                var structureSchema = UpsertStructureSchema<T>();

                var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, Db.ProviderFactory.GetIdentityStructureIdGenerator(CheckOutAndGetNextIdentity));
                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                structureInserter.Insert(structureSchema, new[] { structureBuilder.CreateStructure(realItem, structureSchema) });
            });
        }

        public virtual void InsertJson<T>(string json) where T : class
        {
            Try(() =>
            {
                Ensure.That(json, "json").IsNotNullOrWhiteSpace();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var item = Db.Serializer.Deserialize<T>(json);
                var structureSchema = UpsertStructureSchema<T>();

                var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, Db.ProviderFactory.GetIdentityStructureIdGenerator(CheckOutAndGetNextIdentity));
                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                structureInserter.Insert(structureSchema, new[] { structureBuilder.CreateStructure(item, structureSchema) });
            });
        }

        public virtual void InsertMany<T>(IEnumerable<T> items) where T : class
        {
            Try(() =>
            {
                Ensure.That(items, "items").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = UpsertStructureSchema<T>();

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
            Try(() =>
            {
                Ensure.That(json, "json").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = UpsertStructureSchema<T>();

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
            Try(() =>
            {
                Ensure.That(item, "item").IsNotNull();

                var structureSchema = UpsertStructureSchema<T>();
                var structureId = structureSchema.IdAccessor.GetValue(item);

                if (!structureSchema.HasConcurrencyToken)
                {
                    var exists = TransactionalDbClient.Exists(structureId, structureSchema);
                    if (!exists)
                        throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));
                }
                else
                    EnsureConcurrencyTokenIsValid(structureSchema, structureId, item);

                Db.CacheProvider.NotifyDeleting(structureSchema, structureId);
                TransactionalDbClient.DeleteById(structureId, structureSchema);

                var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
                var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

                var bulkInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                bulkInserter.Insert(structureSchema, new[] { updatedStructure });
            });
        }

        public virtual void Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class
        {
            Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();
                Ensure.That(modifier, "modifier").IsNotNull();

                var structureSchema = UpsertStructureSchema<T>();
                var structureId = StructureId.ConvertFrom(id);

                var existingJson = TransactionalDbClient.GetJsonByIdWithLock(structureId, structureSchema);

                if (string.IsNullOrWhiteSpace(existingJson))
                    throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));
                var item = Db.Serializer.Deserialize<T>(existingJson);

                modifier.Invoke(item);
                if(proceed != null && !proceed.Invoke(item))
                    return;

                if (structureSchema.HasConcurrencyToken)
                    EnsureConcurrencyTokenIsValid(structureSchema, structureId, item);

                Db.CacheProvider.NotifyDeleting(structureSchema, structureId);
                TransactionalDbClient.DeleteById(structureId, structureSchema);

                var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
                var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

                var bulkInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                bulkInserter.Insert(structureSchema, new[] { updatedStructure });
            });
        }

        protected virtual void EnsureConcurrencyTokenIsValid<T>(IStructureSchema structureSchema, IStructureId structureId, T newItem) where T : class
        {
            var existingJson = TransactionalDbClient.GetJsonById(structureId, structureSchema);

            if (string.IsNullOrWhiteSpace(existingJson))
                throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));

            var existingItem = Db.Serializer.Deserialize<T>(existingJson);
            var existingToken = structureSchema.ConcurrencyTokenAccessor.GetValue(existingItem);
            var updatingToken = structureSchema.ConcurrencyTokenAccessor.GetValue(newItem);

            if (!Equals(updatingToken, existingToken))
                throw new SisoDbConcurrencyException(structureId.Value, structureSchema.Name, ExceptionMessages.ConcurrencyException);

            if (existingToken is Guid)
            {
                structureSchema.ConcurrencyTokenAccessor.SetValue(newItem, Guid.NewGuid());
                return;
            }

            if (existingToken is int)
            {
                var existingNumericToken = (int)existingToken;
                structureSchema.ConcurrencyTokenAccessor.SetValue(newItem, existingNumericToken + 1);
                return;
            }

            if (existingToken is long)
            {
                var existingNumericToken = (long)existingToken;
                structureSchema.ConcurrencyTokenAccessor.SetValue(newItem, existingNumericToken + 1);
                return;
            }

            throw new SisoDbException(ExceptionMessages.ConcurrencyTokenIsOfWrongType);
        }

        public virtual void DeleteById<T>(object id) where T : class
        {
            Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = UpsertStructureSchema<T>();

                Db.CacheProvider.NotifyDeleting(structureSchema, structureId);
                TransactionalDbClient.DeleteById(structureId, structureSchema);
            });
        }

        public virtual void DeleteByIds<T>(params object[] ids) where T : class
        {
            Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
                var structureSchema = UpsertStructureSchema<T>();

                Db.CacheProvider.NotifyDeleting(structureSchema, structureIds);
                TransactionalDbClient.DeleteByIds(structureIds, structureSchema);
            });
        }
    }
}