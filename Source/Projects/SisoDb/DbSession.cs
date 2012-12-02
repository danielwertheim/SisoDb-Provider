using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.Caching;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.NCore.Collections;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public abstract class DbSession : ISession
    {
        private readonly Guid _id;
        private readonly ISisoDatabase _db;
        private readonly IQueryEngine _queryEngine;
        private readonly IAdvanced _advanced;
        protected readonly SessionEvents InternalEvents;
        protected readonly IDbQueryGenerator QueryGenerator;
        protected readonly ISqlExpressionBuilder SqlExpressionBuilder;
        protected readonly ISqlStatements SqlStatements;
        
        public Guid Id { get { return _id; } }
        public ISessionExecutionContext ExecutionContext { get; private set; }
        public ISisoDatabase Db { get { return _db; } }
        public IDbClient DbClient { get; private set; }
        public SessionStatus Status { get; private set; }
        public bool IsAborted { get { return Status.IsAborted(); } }
        public bool HasFailed { get { return Status.IsFailed(); } }
        public ISessionEvents Events { get { return InternalEvents; } }
        public IQueryEngine QueryEngine { get { return _queryEngine; } }
        public IAdvanced Advanced { get { return _advanced; } }
        public CacheConsumeModes CacheConsumeMode { get; protected set; }
        
        protected DbSession(ISisoDatabase db)
        {
            Ensure.That(db, "db").IsNotNull();

            _id = Guid.NewGuid();
            _db = db;
            DbClient = Db.ProviderFactory.GetTransactionalDbClient(Db);
            ExecutionContext = new SessionExecutionContext(this);
            Status = SessionStatus.Active;
            InternalEvents = new SessionEvents();
            SqlStatements = Db.ProviderFactory.GetSqlStatements();
            QueryGenerator = Db.ProviderFactory.GetDbQueryGenerator();
            SqlExpressionBuilder = Db.ProviderFactory.GetSqlExpressionBuilder();
            _queryEngine = new DbQueryEngine(ExecutionContext, QueryGenerator);
            _advanced = new DbSessionAdvanced(ExecutionContext, QueryGenerator, SqlExpressionBuilder);
            CacheConsumeMode = CacheConsumeModes.UpdateCacheWithDbResult;
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);

            if (Status.IsDisposed())
                throw new ObjectDisposedException(typeof(DbSession).Name, ExceptionMessages.Session_AllreadyDisposed.Inject(Id, Db.Name));

            if (DbClient.IsFailed || Status.IsFailed())
                Status = SessionStatus.DisposedWithFailure;
            else if (DbClient.IsAborted || Status.IsAborted())
                Status = SessionStatus.DisposedAfterAbort;
            else
                Status = SessionStatus.Disposed;

            if (DbClient != null)
            {
                DbClient.Dispose();
                DbClient = null;
            }

            if(Status.IsAborted() || Status.IsFailed())
                InternalEvents.NotifyRolledback(Db, Id);
            else
                InternalEvents.NotifyCommitted(Db, Id);
        }

        public virtual void Abort()
        {
            if (HasFailed) return;
            //This method is allowed to not be wrapped in Try, since try makes use of it.
            Status = SessionStatus.Aborted;
            DbClient.Abort();
        }

        public virtual void MarkAsFailed()
        {
            //This method is allowed to not be wrapped in Try, since try makes use of it.
            Status = SessionStatus.Failed;
            DbClient.MarkAsFailed();
        }

        protected virtual void Try(Action action)
        {
            ExecutionContext.Try(action);
        }

        protected virtual T Try<T>(Func<T> action)
        {
            return ExecutionContext.Try(action);
        }

        protected virtual IStructureSchema OnUpsertStructureSchema<T>() where T : class
        {
            return OnUpsertStructureSchema(typeof(T));
        }

        protected virtual IStructureSchema OnUpsertStructureSchema(Type structuretype)
        {
            var structureSchema = Db.StructureSchemas.GetSchema(structuretype);
            Db.DbSchemas.Upsert(structureSchema, DbClient);

            return structureSchema;
        }

        public virtual IStructureSchema GetStructureSchema<T>() where T : class
        {
            return Try(() => OnGetStructureSchema(typeof(T)));
        }

        public virtual IStructureSchema GetStructureSchema(Type structureType)
        {
            return Try(() => OnGetStructureSchema(structureType));
        }

        protected virtual IStructureSchema OnGetStructureSchema(Type structureType)
        {
            return Db.StructureSchemas.GetSchema(structureType);
        }

        public virtual ISisoQueryable<T> Query<T>() where T : class
        {
            return Try(() => new SisoQueryable<T>(Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas), QueryEngine));
        }

        public virtual bool Any<T>() where T : class
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return QueryEngine.Any<T>();
        }

        public virtual bool Any(Type structureType)
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return QueryEngine.Any(structureType);
        }

        public virtual int Count<T>() where T : class
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return QueryEngine.Count<T>();
        }

        public virtual int Count(Type structureType)
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return QueryEngine.Count(structureType);
        }

        public virtual bool Exists<T>(object id) where T : class
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return QueryEngine.Exists<T>(id);
        }

        public virtual bool Exists(Type structureType, object id)
        {
            //OK, to not be wrapped in Try, since QueryEngine does this
            return QueryEngine.Exists(structureType, id);
        }

        public virtual T CheckoutById<T>(object id) where T : class
        {
            return Try(() => OnCheckoutById<T>(id));
        }

        protected virtual T OnCheckoutById<T>(object id) where T : class
        {
            Ensure.That(id, "id").IsNotNull();

            var structureId = StructureId.ConvertFrom(id);
            var structureSchema = OnUpsertStructureSchema<T>();

            return Db.CacheProvider.Consume(
                structureSchema,
                structureId,
                sid => Db.Serializer.Deserialize<T>(DbClient.GetJsonByIdWithLock(sid, structureSchema)),
                CacheConsumeMode);
        }

        public virtual T GetByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Try(() => OnGetByQueryAs(typeof(T), predicate));
        }

        protected virtual TOut OnGetByQueryAs<TOut>(Type structureType, Expression<Func<TOut, bool>> predicate)
            where TOut : class
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(predicate, "predicate").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);

            return Db.CacheProvider.Consume(
                structureSchema,
                predicate,
                e =>
                {
                    var queryBuilder = Db.ProviderFactory.GetQueryBuilder<TOut>(Db.StructureSchemas);
                    queryBuilder.Where(predicate);
                    var query = queryBuilder.Build();

                    var sqlQuery = QueryGenerator.GenerateQuery(query);
                    var sourceData = DbClient.ReadJson(structureSchema, sqlQuery.Sql, sqlQuery.Parameters).SingleOrDefault();

                    return Db.Serializer.Deserialize<TOut>(sourceData);
                },
                CacheConsumeMode);
        }

        public virtual T GetById<T>(object id) where T : class
        {
            return Try(() => OnGetByIdAs<T>(typeof(T), id));
        }

        public virtual object GetById(Type structureType, object id)
        {
            return Try(() => OnGetById(structureType, id));
        }

        protected virtual object OnGetById(Type structureType, object id)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(id, "id").IsNotNull();

            var structureId = StructureId.ConvertFrom(id);
            var structureSchema = OnUpsertStructureSchema(structureType);

            return Db.CacheProvider.Consume(
                structureSchema,
                structureId,
                sid => Db.Serializer.Deserialize(DbClient.GetJsonById(sid, structureSchema), structureType),
                CacheConsumeMode);
        }

        public virtual TOut GetByIdAs<TContract, TOut>(object id)
            where TContract : class
            where TOut : class
        {
            return Try(() => OnGetByIdAs<TOut>(typeof(TContract), id));
        }

        public TOut GetByIdAs<TOut>(Type structureType, object id) where TOut : class
        {
            return Try(() => OnGetByIdAs<TOut>(structureType, id));
        }

        protected virtual TOut OnGetByIdAs<TOut>(Type structureType, object id)
            where TOut : class
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(id, "id").IsNotNull();

            var structureId = StructureId.ConvertFrom(id);
            var structureSchema = OnUpsertStructureSchema(structureType);

            return Db.CacheProvider.Consume(
                structureSchema,
                structureId,
                sid => Db.Serializer.Deserialize<TOut>(DbClient.GetJsonById(sid, structureSchema)),
                CacheConsumeMode);
        }

        public virtual IEnumerable<T> GetByIds<T>(params object[] ids) where T : class
        {
            return Try(() => OnGetByIdsAs<T>(typeof(T), ids));
        }

        public virtual IEnumerable<object> GetByIds(Type structureType, params object[] ids)
        {
            return Try(() => OnGetByIds(structureType, ids));
        }

        protected virtual IEnumerable<object> OnGetByIds(Type structureType, params object[] ids)
        {
            Ensure.That(ids, "ids").HasItems();

            var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
            var structureSchema = OnUpsertStructureSchema(structureType);

            return Db.CacheProvider.Consume(
                structureSchema,
                structureIds,
                sids => Db.Serializer.DeserializeMany(DbClient.GetJsonByIds(sids, structureSchema).Where(s => s != null), structureType),
                CacheConsumeMode);
        }

        public virtual IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class
        {
            return Try(() => OnGetByIdsAs<TOut>(typeof(TContract), ids));
        }

        public virtual IEnumerable<TOut> GetByIdsAs<TOut>(Type structureType, params object[] ids)
            where TOut : class
        {
            return Try(() => OnGetByIdsAs<TOut>(structureType, ids));
        }

        protected virtual IEnumerable<TOut> OnGetByIdsAs<TOut>(Type structureType, params object[] ids)
            where TOut : class
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(ids, "ids").HasItems();

            var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
            var structureSchema = OnUpsertStructureSchema(structureType);

            return Db.CacheProvider.Consume(
                structureSchema,
                structureIds,
                sids => Db.Serializer.DeserializeMany<TOut>(DbClient.GetJsonByIds(sids, structureSchema).Where(s => s != null)),
                CacheConsumeMode);
        }

        public virtual string GetByIdAsJson<T>(object id) where T : class
        {
            return Try(() => OnGetByIdAsJson(typeof(T), id));
        }

        public virtual string GetByIdAsJson(Type structureType, object id)
        {
            return Try(() => OnGetByIdAsJson(structureType, id));
        }

        protected virtual string OnGetByIdAsJson(Type structureType, object id)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(id, "id").IsNotNull();

            var structureId = StructureId.ConvertFrom(id);
            var structureSchema = OnUpsertStructureSchema(structureType);

            if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                return DbClient.GetJsonById(structureId, structureSchema);

            var item = Db.CacheProvider.Consume(
                structureSchema,
                structureId,
                sid => Db.Serializer.Deserialize(
                    DbClient.GetJsonById(sid, structureSchema),
                    structureSchema.Type.Type),
                CacheConsumeMode);

            return Db.Serializer.Serialize(item);
        }

        public virtual IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class
        {
            return Try(() => OnGetByIdsAsJson(typeof(T), ids));
        }

        public virtual IEnumerable<string> GetByIdsAsJson(Type structureType, params object[] ids)
        {
            return Try(() => OnGetByIdsAsJson(structureType, ids));
        }

        protected virtual IEnumerable<string> OnGetByIdsAsJson(Type structureType, params object[] ids)
        {
            Ensure.That(ids, "ids").HasItems();

            var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
            var structureSchema = OnUpsertStructureSchema(structureType);

            if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                return DbClient.GetJsonByIds(structureIds, structureSchema).Where(s => s != null);

            var items = Db.CacheProvider.Consume(
                structureSchema,
                structureIds,
                sids => Db.Serializer.DeserializeMany(
                    DbClient.GetJsonByIds(sids, structureSchema),
                    structureSchema.Type.Type).Where(s => s != null),
                CacheConsumeMode);

            return Db.Serializer.SerializeMany(items);
        }

        public virtual ISession Insert<T>(T item) where T : class
        {
            Try(() => OnInsert(typeof(T), item));

            return this;
        }

        public virtual ISession Insert(Type structureType, object item)
        {
            Try(() => OnInsert(structureType, item));

            return this;
        }

        protected virtual void OnInsert(Type structureType, object item)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(item, "item").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);
            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.CleanQueriesFor(structureSchema);

            var structureBuilder = Db.StructureBuilders.ResolveBuilderForInsert(structureSchema, DbClient);
            var structure = structureBuilder.CreateStructure(item, structureSchema);
            
            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            structureInserter.Insert(structureSchema, new[] { structure });
            InternalEvents.NotifyInserted(this, structureSchema, structure, item);
        }

        public virtual ISession InsertAs<T>(object item) where T : class
        {
            Try(() => OnInsertAs(typeof(T), item));

            return this;
        }

        public virtual ISession InsertAs(Type structureType, object item)
        {
            Try(() => OnInsertAs(structureType, item));

            return this;
        }

        protected virtual void OnInsertAs(Type structureType, object item)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(item, "item").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);
            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.CleanQueriesFor(structureSchema);

            var json = Db.Serializer.Serialize(item);
            var realItem = Db.Serializer.Deserialize(json, structureType);
            
            var structureBuilder = Db.StructureBuilders.ResolveBuilderForInsert(structureSchema, DbClient);
            var structure = structureBuilder.CreateStructure(realItem, structureSchema);
            
            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            structureInserter.Insert(structureSchema, new[] { structure });
            InternalEvents.NotifyInserted(this, structureSchema, structure, item);
        }

        public virtual string InsertJson<T>(string json) where T : class
        {
            return Try(() => OnInsertJson(typeof(T), json));
        }

        public virtual string InsertJson(Type structureType, string json)
        {
            return Try(() => OnInsertJson(structureType, json));
        }

        protected virtual string OnInsertJson(Type structureType, string json)
        {
            Ensure.That(json, "json").IsNotNullOrWhiteSpace();

            var structureSchema = OnUpsertStructureSchema(structureType);
            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.CleanQueriesFor(structureSchema);

            var item = Db.Serializer.Deserialize(json, structureType);
            var structureBuilder = Db.StructureBuilders.ResolveBuilderForInsert(structureSchema, DbClient);
            var structure = structureBuilder.CreateStructure(item, structureSchema);

            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            structureInserter.Insert(structureSchema, new[] { structure });
            InternalEvents.NotifyInserted(this, structureSchema, structure, item);

            return structure.Data;
        }

        public virtual ISession InsertMany<T>(IEnumerable<T> items) where T : class
        {
            Try(() => OnInsertMany(typeof(T), items));
            return this;
        }

        public virtual ISession InsertMany(Type structureType, IEnumerable<object> items)
        {
            Try(() => OnInsertMany(structureType, items));
            return this;
        }

        protected virtual void OnInsertMany(Type structureType, IEnumerable<object> items)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(items, "items").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);
            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.CleanQueriesFor(structureSchema);
            
            var structureBuilder = Db.StructureBuilders.ResolveBuilderForInsert(structureSchema, DbClient);
            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);

            foreach (var itemsBatch in items.Batch(Db.Settings.MaxInsertManyBatchSize))
            {
                var structures = structureBuilder.CreateStructures(itemsBatch, structureSchema);
                structureInserter.Insert(structureSchema, structures);
                InternalEvents.NotifyInserted(this, structureSchema, structures, itemsBatch);
            }
        }

        public virtual void InsertManyJson<T>(IEnumerable<string> json) where T : class
        {
            Try(() => OnInsertManyJson(typeof(T), json));
        }

        public virtual void InsertManyJson(Type structureType, IEnumerable<string> json)
        {
            Try(() => OnInsertManyJson(structureType, json));
        }

        protected virtual void OnInsertManyJson(Type structureType, IEnumerable<string> json)
        {
            Ensure.That(json, "json").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);
            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.CleanQueriesFor(structureSchema);

            var structureBuilder = Db.StructureBuilders.ResolveBuilderForInsert(structureSchema, DbClient);
            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);

            foreach (var itemsBatch in Db.Serializer.DeserializeMany(json, structureSchema.Type.Type).Batch(Db.Settings.MaxInsertManyBatchSize))
            {
                var structures = structureBuilder.CreateStructures(itemsBatch, structureSchema);
                structureInserter.Insert(structureSchema, structures);
                InternalEvents.NotifyInserted(this, structureSchema, structures, itemsBatch);
            }
        }

        public virtual ISession Update<T>(T item) where T : class
        {
            Try(() => OnUpdate(typeof(T), item));

            return this;
        }

        public virtual ISession Update(Type structureType, object item)
        {
            Try(() => OnUpdate(structureType, item));

            return this;
        }

        protected virtual void OnUpdate(Type structureType, object item)
        {
            Ensure.That(item, "item").IsNotNull();

            var implType = item.GetType();
            var structureSchema = OnUpsertStructureSchema(structureType);
            var structureId = structureSchema.IdAccessor.GetValue(item);

            if (!structureSchema.HasConcurrencyToken)
            {
                var exists = DbClient.Exists(structureSchema, structureId);
                if (!exists)
                    throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));
            }
            else
                OnEnsureConcurrencyTokenIsValid(structureSchema, structureId, item, implType);

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.CleanQueriesFor(structureSchema);
            Db.CacheProvider.Remove(structureSchema, structureId);
            DbClient.DeleteIndexesAndUniquesById(structureId, structureSchema);

            var structureBuilder = Db.StructureBuilders.ResolveBuilderForUpdate(structureSchema);
            var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

            var bulkInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            bulkInserter.Replace(structureSchema, updatedStructure);
            InternalEvents.NotifyUpdated(this, structureSchema, updatedStructure, item);
        }

        public virtual ISession Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class
        {
            Try(() => OnUpdate<T, T>(id, modifier, proceed));

            return this;
        }

        public virtual ISession Update<TContract, TImpl>(object id, Action<TImpl> modifier, Func<TImpl, bool> proceed = null)
            where TContract : class
            where TImpl : class
        {
            Try(() => OnUpdate<TContract, TImpl>(id, modifier, proceed));

            return this;
        }

        protected virtual ISession OnUpdate<TContract, TImpl>(object id, Action<TImpl> modifier, Func<TImpl, bool> proceed = null)
            where TContract : class
            where TImpl : class
        {
            Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();
                Ensure.That(modifier, "modifier").IsNotNull();

                var structureSchema = OnUpsertStructureSchema<TContract>();
                var structureId = StructureId.ConvertFrom(id);

                var existingJson = DbClient.GetJsonByIdWithLock(structureId, structureSchema);

                if (string.IsNullOrWhiteSpace(existingJson))
                    throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));

                var item = Db.Serializer.Deserialize<TImpl>(existingJson);

                modifier.Invoke(item);
                if (proceed != null && !proceed.Invoke(item))
                    return;

                if (structureSchema.HasConcurrencyToken)
                    OnEnsureConcurrencyTokenIsValid(structureSchema, structureId, item, typeof(TImpl));

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
                Db.CacheProvider.CleanQueriesFor(structureSchema);
                Db.CacheProvider.Remove(structureSchema, structureId);
                DbClient.DeleteIndexesAndUniquesById(structureId, structureSchema);

                var structureBuilder = Db.StructureBuilders.ResolveBuilderForUpdate(structureSchema);
                var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

                var bulkInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
                bulkInserter.Replace(structureSchema, updatedStructure);
                InternalEvents.NotifyUpdated(this, structureSchema, updatedStructure, item);
            });

            return this;
        }

        protected virtual void OnEnsureConcurrencyTokenIsValid(IStructureSchema structureSchema, IStructureId structureId, object newItem, Type typeForDeserialization)
        {
            var existingJson = DbClient.GetJsonById(structureId, structureSchema);

            if (string.IsNullOrWhiteSpace(existingJson))
                throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));

            var existingItem = Db.Serializer.Deserialize(existingJson, typeForDeserialization);
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

        public virtual ISession UpdateMany<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class
        {
            Try(() =>
            {
                Ensure.That(predicate, "predicate").IsNotNull();
                Ensure.That(modifier, "modifier").IsNotNull();

                var structureSchema = OnUpsertStructureSchema<T>();
                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
                Db.CacheProvider.CleanQueriesFor(structureSchema);

                var deleteIds = new HashSet<IStructureId>();
                var keepQueue = new List<T>(Db.Settings.MaxUpdateManyBatchSize);
                var structureBuilder = Db.StructureBuilders.ResolveBuilderForUpdate(structureSchema);
                var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
                var query = queryBuilder.Where(predicate).Build();
                var sqlQuery = QueryGenerator.GenerateQuery(query);

                foreach (var structure in Db.Serializer.DeserializeMany<T>(
                    DbClient.ReadJson(structureSchema, sqlQuery.Sql, sqlQuery.Parameters)))
                {
                    var structureIdBefore = structureSchema.IdAccessor.GetValue(structure);
                    modifier.Invoke(structure);
                    var structureIdAfter = structureSchema.IdAccessor.GetValue(structure);

                    if (!structureIdBefore.Value.Equals(structureIdAfter.Value))
                        throw new SisoDbException(ExceptionMessages.WriteSession_UpdateMany_NewIdDoesNotMatchOldId.Inject(
                                structureIdAfter.Value, structureIdBefore.Value));

                    deleteIds.Add(structureIdBefore);

                    keepQueue.Add(structure);
                    if (keepQueue.Count < Db.Settings.MaxUpdateManyBatchSize)
                        continue;

                    Db.CacheProvider.Remove(structureSchema, deleteIds);
                    DbClient.DeleteByIds(deleteIds, structureSchema);
                    deleteIds.Clear();

                    var items = keepQueue.ToArray();
                    var structures = structureBuilder.CreateStructures(items, structureSchema);
                    structureInserter.Insert(structureSchema, structures);
                    keepQueue.Clear();
                    InternalEvents.NotifyUpdated(this, structureSchema, structures, items);
                }

                if (keepQueue.Count > 0)
                {
                    Db.CacheProvider.Remove(structureSchema, deleteIds);
                    DbClient.DeleteByIds(deleteIds, structureSchema);
                    deleteIds.Clear();

                    var items = keepQueue.ToArray();
                    var structures = structureBuilder.CreateStructures(items, structureSchema);
                    structureInserter.Insert(structureSchema, structures);
                    keepQueue.Clear();
                    InternalEvents.NotifyUpdated(this, structureSchema, structures, items);
                }
            });

            return this;
        }

        public virtual ISession Clear<T>() where T : class
        {
            Try(() => OnClear(typeof(T)));

            return this;
        }

        public virtual ISession Clear(Type structureType)
        {
            Try(() => OnClear(structureType));

            return this;
        }

        protected virtual void OnClear(Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.ClearByType(structureType);

            DbClient.DeleteAll(structureSchema);
        }

        public virtual ISession DeleteAllExceptIds<T>(params object[] ids) where T : class
        {
            Try(() => OnDeleteAllExceptIds(typeof(T), ids));

            return this;
        }

        public virtual ISession DeleteAllExceptIds(Type structureType, params object[] ids)
        {
            Try(() => OnDeleteAllExceptIds(structureType, ids));

            return this;
        }

        protected virtual void OnDeleteAllExceptIds(Type structureType, params object[] ids)
        {
            Ensure.That(ids, "ids").HasItems();
            Ensure.That(structureType, "structureType").IsNotNull();

            var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
            var structureSchema = OnUpsertStructureSchema(structureType);

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.ClearByType(structureType);

            DbClient.DeleteAllExceptIds(structureIds, structureSchema);
        }

        public virtual ISession DeleteById<T>(object id) where T : class
        {
            Try(() => OnDeleteById(typeof(T), id));

            return this;
        }

        public virtual ISession DeleteById(Type structureType, object id)
        {
            Try(() => OnDeleteById(structureType, id));

            return this;
        }

        protected virtual void OnDeleteById(Type structureType, object id)
        {
            Ensure.That(id, "id").IsNotNull();

            var structureId = StructureId.ConvertFrom(id);
            var structureSchema = OnUpsertStructureSchema(structureType);

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.Remove(structureSchema, structureId);

            DbClient.DeleteById(structureId, structureSchema);
            InternalEvents.NotifyDeleted(this, structureSchema, structureId);
        }

        public virtual ISession DeleteByIds<T>(params object[] ids) where T : class
        {
            Try(() => OnDeleteByIds(typeof(T), ids));

            return this;
        }

        public virtual ISession DeleteByIds(Type structureType, params object[] ids)
        {
            Try(() => OnDeleteByIds(structureType, ids));

            return this;
        }

        protected virtual void OnDeleteByIds(Type structureType, params object[] ids)
        {
            Ensure.That(ids, "ids").HasItems();
            Ensure.That(structureType, "structureType").IsNotNull();

            var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
            var structureSchema = OnUpsertStructureSchema(structureType);

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.Remove(structureSchema, new HashSet<IStructureId>(structureIds));

            DbClient.DeleteByIds(structureIds, structureSchema);
            InternalEvents.NotifyDeleted(this, structureSchema, structureIds);
        }

        public virtual ISession DeleteByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Try(() =>
            {
                Ensure.That(predicate, "predicate").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema<T>();
                Db.CacheProvider.ClearByType(structureSchema);

                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
                queryBuilder.Where(predicate);

                var query = queryBuilder.Build();
                var sql = QueryGenerator.GenerateQueryReturningStrutureIds(query);
                DbClient.DeleteByQuery(sql, structureSchema);
                InternalEvents.NotifyDeleted(this, structureSchema, query);
            });

            return this;
        }
    }
}