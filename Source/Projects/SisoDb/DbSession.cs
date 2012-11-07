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
    public abstract class DbSession : ITransactionalSession
    {
        private readonly Guid _id;
        private readonly ISisoDatabase _db;
        private readonly IQueryEngine _queryEngine;
        private readonly IAdvanced _advanced;
        protected IDbClient DbClient;
        protected readonly IDbQueryGenerator QueryGenerator;
        protected readonly ISqlExpressionBuilder SqlExpressionBuilder;
        protected readonly ISqlStatements SqlStatements;
        protected readonly SessionExecutionContext ExecutionContext;
        protected CacheConsumeModes CacheConsumeMode;

        public Guid Id { get { return _id; } }
        public ISisoDatabase Db { get { return _db; } }
        public SessionStatus Status { get; private set; }
        public IQueryEngine QueryEngine { get { return _queryEngine; } }
        public IAdvanced Advanced { get { return _advanced; } }
        public bool Failed { get { return Status.IsFailed(); } }

        protected DbSession(ISisoDatabase db)
        {
            Ensure.That(db, "db").IsNotNull();

            _id = Guid.NewGuid();
            _db = db;
            ExecutionContext = new SessionExecutionContext(this);
            Status = SessionStatus.Active;
            SqlStatements = Db.ProviderFactory.GetSqlStatements();
            QueryGenerator = Db.ProviderFactory.GetDbQueryGenerator();
            SqlExpressionBuilder = Db.ProviderFactory.GetSqlExpressionBuilder();
            DbClient = Db.ProviderFactory.GetTransactionalDbClient(Db.ConnectionInfo);
            _queryEngine = new DbQueryEngine(Db, DbClient, QueryGenerator, ExecutionContext);
            _advanced = new DbSessionAdvanced(Db, DbClient, QueryGenerator, SqlExpressionBuilder, ExecutionContext);
            CacheConsumeMode = CacheConsumeModes.UpdateCacheWithDbResult;
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);

            if (Status.IsDisposed())
                throw new ObjectDisposedException(typeof(DbSession).Name, ExceptionMessages.Session_AllreadyDisposed.Inject(Id, Db.Name));

            Status = DbClient.IsFailed
                ? SessionStatus.DisposedWithFailure
                : SessionStatus.Disposed;

            if (DbClient != null)
            {
                DbClient.Dispose();
                DbClient = null;
            }
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

        public virtual T GetById<T>(object id) where T : class
        {
            return Try(() => OnGetByIdAs<T, T>(id));
        }

        public virtual object GetById(Type structureType, object id)
        {
            return Try(() => OnGetById(structureType, id));
        }

        protected virtual object OnGetById(Type structureType, object id)
        {
            Ensure.That(id, "id").IsNotNull();

            var structureId = StructureId.ConvertFrom(id);
            var structureSchema = OnUpsertStructureSchema(structureType);

            if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                return Db.Serializer.Deserialize(DbClient.GetJsonById(structureId, structureSchema), structureType);

            return Db.CacheProvider.Consume(
                structureSchema,
                structureId,
                sid => Db.Serializer.Deserialize(DbClient.GetJsonById(sid, structureSchema), structureType),
                CacheConsumeMode);
        }

        public virtual IEnumerable<T> GetByIds<T>(params object[] ids) where T : class
        {
            return Try(() => OnGetByIdsAs<T, T>(ids));
        }

        public virtual object[] GetByIds(Type structureType, params object[] ids)
        {
            return Try(() => OnGetByIds(structureType, ids));
        }

        protected virtual object[] OnGetByIds(Type structureType, params object[] ids)
        {
            Ensure.That(ids, "ids").HasItems();

            var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
            var structureSchema = OnUpsertStructureSchema(structureType);

            if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                return Db.Serializer.DeserializeMany(DbClient.GetJsonByIds(structureIds, structureSchema).Where(s => s != null).ToArray(), structureType).ToArray();

            return Db.CacheProvider.Consume(
                structureSchema,
                structureIds,
                sids => Db.Serializer.DeserializeMany(DbClient.GetJsonByIds(sids, structureSchema).Where(s => s != null).ToArray(), structureType),
                CacheConsumeMode).ToArray();
        }

        public virtual TOut GetByIdAs<TContract, TOut>(object id)
            where TContract : class
            where TOut : class
        {
            return Try(() => OnGetByIdAs<TContract, TOut>(id));
        }

        protected virtual TOut OnGetByIdAs<TContract, TOut>(object id)
            where TContract : class
            where TOut : class
        {
            Ensure.That(id, "id").IsNotNull();

            var structureId = StructureId.ConvertFrom(id);
            var structureSchema = OnUpsertStructureSchema<TContract>();

            if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                return Db.Serializer.Deserialize<TOut>(DbClient.GetJsonById(structureId, structureSchema));

            return Db.CacheProvider.Consume(
                structureSchema,
                structureId,
                sid => Db.Serializer.Deserialize<TOut>(DbClient.GetJsonById(sid, structureSchema)),
                CacheConsumeMode);
        }

        public virtual IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class
        {
            return Try(() => OnGetByIdsAs<TContract, TOut>(ids));
        }

        protected virtual IEnumerable<TOut> OnGetByIdsAs<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class
        {
            Ensure.That(ids, "ids").HasItems();

            var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
            var structureSchema = OnUpsertStructureSchema<TContract>();

            if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                return Db.Serializer.DeserializeMany<TOut>(DbClient.GetJsonByIds(structureIds, structureSchema).Where(s => s != null).ToArray());

            return Db.CacheProvider.Consume(
                structureSchema,
                structureIds,
                sids => Db.Serializer.DeserializeMany<TOut>(DbClient.GetJsonByIds(sids, structureSchema).Where(s => s != null).ToArray()),
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
                return DbClient.GetJsonByIds(structureIds, structureSchema).Where(s => s != null).ToArray();

            var items = Db.CacheProvider.Consume(
                structureSchema,
                structureIds,
                sids => Db.Serializer.DeserializeMany(
                    DbClient.GetJsonByIds(sids, structureSchema),
                    structureSchema.Type.Type).Where(s => s != null).ToArray(),
                CacheConsumeMode);

            return Db.Serializer.SerializeMany(items).ToArray();
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

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

            var structureSchema = OnUpsertStructureSchema(structureType);
            var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, DbClient);

            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            structureInserter.Insert(structureSchema, new[] { structureBuilder.CreateStructure(item, structureSchema) });
        }

        public virtual ISession InsertAs<T>(object item) where T : class
        {
            Try(() =>
            {
                Ensure.That(item, "item").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema<T>();
                var json = Db.Serializer.Serialize(item); //TODO: Unnecessary
                var realItem = Db.Serializer.Deserialize<T>(json);

                var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, DbClient);
                var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
                structureInserter.Insert(structureSchema, new[] { structureBuilder.CreateStructure(realItem, structureSchema) });
            });

            return this;
        }

        public virtual ISession InsertAs(Type structureType, object item)
        {
            Try(() =>
            {
                Ensure.That(structureType, "structureType").IsNotNull();
                Ensure.That(item, "item").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema(structureType);
                var json = Db.Serializer.Serialize(item);
                var realItem = Db.Serializer.Deserialize(json, structureType);
                var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, DbClient);

                var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
                structureInserter.Insert(structureSchema, new[] { structureBuilder.CreateStructure(realItem, structureSchema) });
            });

            return this;
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

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

            var item = Db.Serializer.Deserialize(json, structureType);
            var structureSchema = OnUpsertStructureSchema(structureType);
            var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, DbClient);
            var structure = structureBuilder.CreateStructure(item, structureSchema);

            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            structureInserter.Insert(structureSchema, new[] { structure });

            return structure.Data;
        }

        public virtual ISession InsertMany<T>(IEnumerable<T> items) where T : class
        {
            Try(() =>
            {
                Ensure.That(items, "items").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema<T>();
                var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, DbClient);
                var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);

                foreach (var structuresBatch in items.Batch(Db.Settings.MaxInsertManyBatchSize))
                    structureInserter.Insert(structureSchema, structureBuilder.CreateStructures(structuresBatch, structureSchema));
            });

            return this;
        }

        public virtual ISession InsertMany(Type structureType, IEnumerable<object> items)
        {
            Try(() =>
            {
                Ensure.That(structureType, "structureType").IsNotNull();
                Ensure.That(items, "items").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema(structureType);
                var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, DbClient);
                var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);

                foreach (var structuresBatch in items.Batch(Db.Settings.MaxInsertManyBatchSize))
                    structureInserter.Insert(structureSchema, structureBuilder.CreateStructures(structuresBatch, structureSchema));
            });
            return this;
        }

        public virtual void InsertManyJson<T>(IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null) where T : class
        {
            Try(() => OnInsertManyJson(typeof(T), json, onBatchInserted));
        }

        public virtual void InsertManyJson(Type structureType, IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null)
        {
            Try(() => OnInsertManyJson(structureType, json, onBatchInserted));
        }

        protected virtual void OnInsertManyJson(Type structureType, IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null)
        {
            Ensure.That(json, "json").IsNotNull();

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

            var structureSchema = OnUpsertStructureSchema(structureType);
            var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, DbClient);
            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);

            foreach (var structuresJsonBatch in Db.Serializer.DeserializeMany(json, structureSchema.Type.Type).Batch(Db.Settings.MaxInsertManyBatchSize))
            {
                var structures = structureBuilder.CreateStructures(structuresJsonBatch, structureSchema);
                structureInserter.Insert(structureSchema, structures);
                if (onBatchInserted != null)
                    onBatchInserted.Invoke(structures.Select(s => s.Data));
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
            Db.CacheProvider.NotifyDeleting(structureSchema, structureId);
            DbClient.DeleteIndexesAndUniquesById(structureId, structureSchema);

            var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
            var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

            var bulkInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            bulkInserter.Replace(structureSchema, updatedStructure);
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
                Db.CacheProvider.NotifyDeleting(structureSchema, structureId);
                DbClient.DeleteIndexesAndUniquesById(structureId, structureSchema);

                var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
                var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

                var bulkInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
                bulkInserter.Replace(structureSchema, updatedStructure);
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

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema<T>();

                var deleteIds = new List<IStructureId>(Db.Settings.MaxUpdateManyBatchSize);
                var keepQueue = new List<T>(Db.Settings.MaxUpdateManyBatchSize);
                var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
                var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
                var query = queryBuilder.Where(predicate).Build();
                var sqlQuery = QueryGenerator.GenerateQuery(query);

                foreach (var structure in Db.Serializer.DeserializeMany<T>(
                    DbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray())))
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

                    Db.CacheProvider.NotifyDeleting(structureSchema, deleteIds);
                    DbClient.DeleteByIds(deleteIds, structureSchema);
                    deleteIds.Clear();

                    structureInserter.Insert(structureSchema,
                                             structureBuilder.CreateStructures(keepQueue.ToArray(), structureSchema));
                    keepQueue.Clear();
                }

                if (keepQueue.Count > 0)
                {
                    Db.CacheProvider.NotifyDeleting(structureSchema, deleteIds);
                    DbClient.DeleteByIds(deleteIds, structureSchema);
                    deleteIds.Clear();

                    structureInserter.Insert(structureSchema,
                                             structureBuilder.CreateStructures(keepQueue.ToArray(), structureSchema));
                    keepQueue.Clear();
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
            Db.CacheProvider.NotifyOfPurge(structureType);

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
            Db.CacheProvider.NotifyOfPurge(structureType);

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
            Db.CacheProvider.NotifyDeleting(structureSchema, structureId);

            DbClient.DeleteById(structureId, structureSchema);
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
            Db.CacheProvider.NotifyDeleting(structureSchema, structureIds);

            DbClient.DeleteByIds(structureIds, structureSchema);
        }

        public virtual ISession DeleteByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Try(() =>
            {
                Ensure.That(predicate, "predicate").IsNotNull();

                CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

                var structureSchema = OnUpsertStructureSchema<T>();
                Db.CacheProvider.NotifyOfPurge(structureSchema);

                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
                queryBuilder.Where(predicate);

                var sql = QueryGenerator.GenerateQueryReturningStrutureIds(queryBuilder.Build());
                DbClient.DeleteByQuery(sql, structureSchema);
            });

            return this;
        }
    }
}