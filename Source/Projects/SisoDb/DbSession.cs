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

        protected virtual long OnCheckOutAndGetNextIdentity(IStructureSchema structureSchema, int numOfIds)
        {
            return NonTransactionalDbClient.CheckOutAndGetNextIdentity(structureSchema.Name, numOfIds);
        }

        protected virtual IStructureSchema OnUpsertStructureSchema<T>() where T : class
        {
            return OnUpsertStructureSchema(TypeFor<T>.Type);
        }

        protected virtual IStructureSchema OnUpsertStructureSchema(Type structuretype)
        {
            var structureSchema = Db.StructureSchemas.GetSchema(structuretype);
            Db.SchemaManager.UpsertStructureSet(structureSchema, NonTransactionalDbClient);
            return structureSchema;
        }

        public virtual IStructureSchema GetStructureSchema<T>() where T : class
        {
            return Try(() => OnGetStructureSchema(TypeFor<T>.Type));
        }

        public virtual IStructureSchema GetStructureSchema(Type structureType)
        {
            return Try(() => OnGetStructureSchema(structureType));
        }

        protected virtual IStructureSchema OnGetStructureSchema(Type structureType)
        {
            return StructureSchemas.GetSchema(structureType);
        }

        void IAdvanced.DeleteByQuery<T>(Expression<Func<T, bool>> predicate)
        {
            Try(() =>
            {
                Ensure.That(predicate, "predicate").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema<T>();
                Db.CacheProvider.NotifyOfPurge(structureSchema);

                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(StructureSchemas);
                queryBuilder.Where(predicate);

                var sql = QueryGenerator.GenerateQueryReturningStrutureIds(queryBuilder.Build());
                TransactionalDbClient.DeleteByQuery(sql, structureSchema);
            });
        }

        IEnumerable<T> IAdvanced.NamedQuery<T>(INamedQuery query)
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                OnUpsertStructureSchema<T>();

                var sourceData = TransactionalDbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray());

                return Db.Serializer.DeserializeMany<T>(sourceData.ToArray());
            });
        }

        IEnumerable<TOut> IAdvanced.NamedQueryAs<TContract, TOut>(INamedQuery query)
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                OnUpsertStructureSchema<TContract>();

                var sourceData = TransactionalDbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray());

                return Db.Serializer.DeserializeMany<TOut>(sourceData.ToArray());
            });
        }

        IEnumerable<string> IAdvanced.NamedQueryAsJson<T>(INamedQuery query)
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                OnUpsertStructureSchema<T>();

                return TransactionalDbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray()).ToArray();
            });
        }

        IEnumerable<T> IAdvanced.RawQuery<T>(IRawQuery query)
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                OnUpsertStructureSchema<T>();

                var sourceData = TransactionalDbClient.YieldJson(query.QueryString, query.Parameters.ToArray());
                return Db.Serializer.DeserializeMany<T>(sourceData.ToArray());
            });
        }

        IEnumerable<TOut> IAdvanced.RawQueryAs<TContract, TOut>(IRawQuery query)
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                OnUpsertStructureSchema<TContract>();

                var sourceData = TransactionalDbClient.YieldJson(query.QueryString, query.Parameters.ToArray());
                return Db.Serializer.DeserializeMany<TOut>(sourceData.ToArray());
            });
        }

        IEnumerable<string> IAdvanced.RawQueryAsJson<T>(IRawQuery query)
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                OnUpsertStructureSchema<T>();

                return TransactionalDbClient.YieldJson(query.QueryString, query.Parameters.ToArray()).ToArray();
            });
        }

        void IAdvanced.UpdateMany<T>(Expression<Func<T, bool>> predicate, Action<T> modifier)
        {
            Try(() =>
            {
                Ensure.That(predicate, "predicate").IsNotNull();
                Ensure.That(modifier, "modifier").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema<T>();

                var deleteIds = new List<IStructureId>(MaxUpdateManyBatchSize);
                var keepQueue = new List<T>(MaxUpdateManyBatchSize);
                var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(StructureSchemas);
                var query = queryBuilder.Where(predicate).Build();
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

                var structureSchema = OnUpsertStructureSchema<T>();

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
                var structureSchema = OnUpsertStructureSchema<T>();

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
                var structureSchema = OnUpsertStructureSchema<T>();

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return Db.Serializer.Deserialize<T>(TransactionalDbClient.GetJsonById(structureId, structureSchema));

                return Db.CacheProvider.Consume(
                    structureSchema,
                    structureId,
                    sid => Db.Serializer.Deserialize<T>(TransactionalDbClient.GetJsonById(sid, structureSchema)),
                    CacheConsumeMode);
            });
        }

        public virtual IEnumerable<T> GetByIds<T>(params object[] ids) where T : class
        {
            return Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
                var structureSchema = OnUpsertStructureSchema<T>();

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return Db.Serializer.DeserializeMany<T>(TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).Where(s => s != null).ToArray());

                return Db.CacheProvider.Consume(
                    structureSchema,
                    structureIds,
                    sids => Db.Serializer.DeserializeMany<T>(TransactionalDbClient.GetJsonByIds(sids, structureSchema).Where(s => s != null).ToArray()),
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
                var structureSchema = OnUpsertStructureSchema<TContract>();

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return Db.Serializer.Deserialize<TOut>(TransactionalDbClient.GetJsonById(structureId, structureSchema));

                return Db.CacheProvider.Consume(
                    structureSchema,
                    structureId,
                    sid => Db.Serializer.Deserialize<TOut>(TransactionalDbClient.GetJsonById(sid, structureSchema)),
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
                var structureSchema = OnUpsertStructureSchema<TContract>();

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return Db.Serializer.DeserializeMany<TOut>(TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).Where(s => s != null).ToArray());

                return Db.CacheProvider.Consume(
                    structureSchema,
                    structureIds,
                    sids => Db.Serializer.DeserializeMany<TOut>(TransactionalDbClient.GetJsonByIds(sids, structureSchema).Where(s => s != null).ToArray()),
                    CacheConsumeMode);
            });
        }

        public virtual string GetByIdAsJson<T>(object id) where T : class
        {
            return Try(() => OnGetByIdAsJson(TypeFor<T>.Type, id));
        }

        public virtual string GetByIdAsJson(Type structureType, object id)
        {
            return Try(() => OnGetByIdAsJson(structureType, id));
        }

        protected virtual string OnGetByIdAsJson(Type structureType, object id)
        {
            Ensure.That(id, "id").IsNotNull();

            var structureId = StructureId.ConvertFrom(id);
            var structureSchema = OnUpsertStructureSchema(structureType);

            if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                return TransactionalDbClient.GetJsonById(structureId, structureSchema);

            var item = Db.CacheProvider.Consume(
                structureSchema,
                structureId,
                sid => Db.Serializer.Deserialize(
                    structureType,
                    TransactionalDbClient.GetJsonById(sid, structureSchema)),
                CacheConsumeMode);

            return Db.Serializer.Serialize(item);
        }

        public virtual IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class
        {
            return Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
                var structureSchema = OnUpsertStructureSchema<T>();

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).Where(s => s != null).ToArray();

                var items = Db.CacheProvider.Consume(
                    structureSchema,
                    structureIds,
                    sids => Db.Serializer.DeserializeMany<T>(TransactionalDbClient.GetJsonByIds(sids, structureSchema)).Where(s => s != null).ToArray(),
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

                var structureSchema = OnUpsertStructureSchema<T>();

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

                var structureSchema = OnUpsertStructureSchema<T>();

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

                var structureSchema = OnUpsertStructureSchema<T>();

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
            return Try(() => OnQueryAsJson(TypeFor<T>.Type, query));
        }

        public virtual IEnumerable<string> QueryAsJson(Type structuretype, IQuery query)
        {
            return Try(() => OnQueryAsJson(structuretype, query));
        }

        protected virtual IEnumerable<string> OnQueryAsJson(Type structuretype, IQuery query)
        {
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structuretype);

            if (query.IsEmpty)
                return TransactionalDbClient.GetJsonOrderedByStructureId(structureSchema).ToArray();

            var sqlQuery = QueryGenerator.GenerateQuery(query);

            return TransactionalDbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray()).ToArray();
        }

        public virtual void Insert<T>(T item) where T : class
        {
            Try(() =>
            {
                Ensure.That(item, "item").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema<T>();

                var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, Db.ProviderFactory.GetIdentityStructureIdGenerator(OnCheckOutAndGetNextIdentity));
                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                structureInserter.Insert(structureSchema, new[] { structureBuilder.CreateStructure(item, structureSchema) });
            });
        }

        public virtual void InsertAs<T>(object item) where T : class
        {
            Try(() =>
            {
                Ensure.That(item, "item").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var json = Db.Serializer.Serialize(item);
                var realItem = Db.Serializer.Deserialize<T>(json);

                var structureSchema = OnUpsertStructureSchema<T>();

                var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, Db.ProviderFactory.GetIdentityStructureIdGenerator(OnCheckOutAndGetNextIdentity));
                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                structureInserter.Insert(structureSchema, new[] { structureBuilder.CreateStructure(realItem, structureSchema) });
            });
        }

        public virtual string InsertJson<T>(string json) where T : class
        {
            return Try(() => OnInsertJson(TypeFor<T>.Type, json));
        }

        public virtual string InsertJson(Type structureType, string json)
        {
            return Try(() => OnInsertJson(structureType, json));
        }

        protected virtual string OnInsertJson(Type structureType, string json)
        {
            Ensure.That(json, "json").IsNotNullOrWhiteSpace();

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

            var item = Db.Serializer.Deserialize(structureType, json);
            var structureSchema = OnUpsertStructureSchema(structureType);

            var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, Db.ProviderFactory.GetIdentityStructureIdGenerator(OnCheckOutAndGetNextIdentity));
            var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
            var structure = structureBuilder.CreateStructure(item, structureSchema);
            structureInserter.Insert(structureSchema, new[] { structure });

            return structure.Data;
        }

        public virtual void InsertMany<T>(IEnumerable<T> items) where T : class
        {
            Try(() =>
            {
                Ensure.That(items, "items").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema<T>();

                var structureBuilder = Db.StructureBuilders.ForInserts(
                    structureSchema,
                    Db.ProviderFactory.GetIdentityStructureIdGenerator(OnCheckOutAndGetNextIdentity));

                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                foreach (var structuresBatch in items.Batch(MaxInsertManyBatchSize))
                    structureInserter.Insert(structureSchema, structureBuilder.CreateStructures(structuresBatch, structureSchema));
            });
        }

        public virtual void InsertManyJson<T>(IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null) where T : class
        {
            Try(() =>
            {
                Ensure.That(json, "json").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema<T>();

                var structureBuilder = Db.StructureBuilders.ForInserts(
                    structureSchema,
                    Db.ProviderFactory.GetIdentityStructureIdGenerator(OnCheckOutAndGetNextIdentity));

                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                foreach (var structuresJsonBatch in Db.Serializer.DeserializeMany<T>(json).Batch(MaxInsertManyBatchSize))
                {
                    var structures = structureBuilder.CreateStructures(structuresJsonBatch, structureSchema);
                    structureInserter.Insert(structureSchema, structures);
                    if (onBatchInserted != null)
                        onBatchInserted.Invoke(structures.Select(s => s.Data));
                }
            });
        }

        public virtual void Update<T>(T item) where T : class
        {
            Try(() =>
            {
                Ensure.That(item, "item").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema<T>();
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

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema<T>();
                var structureId = StructureId.ConvertFrom(id);

                var existingJson = TransactionalDbClient.GetJsonByIdWithLock(structureId, structureSchema);

                if (string.IsNullOrWhiteSpace(existingJson))
                    throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));
                var item = Db.Serializer.Deserialize<T>(existingJson);

                modifier.Invoke(item);
                if (proceed != null && !proceed.Invoke(item))
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
            Try(() => OnDeleteById(TypeFor<T>.Type, id));
        }

        public virtual void DeleteById(Type structureType, object id)
        {
            Try(() => OnDeleteById(structureType, id));
        }

        protected virtual void OnDeleteById(Type structureType, object id)
        {
            Ensure.That(id, "id").IsNotNull();

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

            var structureId = StructureId.ConvertFrom(id);
            var structureSchema = OnUpsertStructureSchema(structureType);

            Db.CacheProvider.NotifyDeleting(structureSchema, structureId);
            TransactionalDbClient.DeleteById(structureId, structureSchema);
        }

        public virtual void DeleteByIds<T>(params object[] ids) where T : class
        {
            Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
                var structureSchema = OnUpsertStructureSchema<T>();

                Db.CacheProvider.NotifyDeleting(structureSchema, structureIds);
                TransactionalDbClient.DeleteByIds(structureIds, structureSchema);
            });
        }
    }
}