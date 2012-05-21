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
using SisoDb.Querying.Sql;
using SisoDb.Resources;

namespace SisoDb
{
    //TODO: Use composition instead for e.g IQueryEngine and IAdvanced
    public abstract class DbSession : ISession, IQueryEngine, IAdvanced
    {
        private readonly ISisoDatabase _db;
        protected readonly IDbQueryGenerator QueryGenerator;
        protected readonly ISqlExpressionBuilder SqlExpressionBuilder;
        protected readonly ISqlStatements SqlStatements;
        protected ITransactionalDbClient TransactionalDbClient;
        protected IDbClient NonTransactionalDbClient;
        protected CacheConsumeModes CacheConsumeMode;

        public ISisoDatabase Db
        {
            get { return _db; }
        }

        public IQueryEngine QueryEngine
        {
            get { return this; }
        }

        public IAdvanced Advanced
        {
            get { return this; }
        }

        protected DbSession(ISisoDatabase db)
        {
            Ensure.That(db, "db").IsNotNull();

            _db = db;
            SqlStatements = Db.ProviderFactory.GetSqlStatements();
            QueryGenerator = Db.ProviderFactory.GetDbQueryGenerator();
            SqlExpressionBuilder = Db.ProviderFactory.GetSqlExpressionBuilder();

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
            return TransactionalDbClient.CheckOutAndGetNextIdentity(structureSchema.Name, numOfIds);
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
            return Db.StructureSchemas.GetSchema(structureType);
        }

        void IAdvanced.DeleteByQuery<T>(Expression<Func<T, bool>> predicate)
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
                TransactionalDbClient.DeleteByQuery(sql, structureSchema);
            });
        }

        void IAdvanced.NonQuery(string sql, params IDacParameter[] parameters)
        {
            Try(() =>
            {
                Ensure.That(sql, "sql").IsNotNullOrWhiteSpace();
                TransactionalDbClient.ExecuteNonQuery(sql, parameters);
            });
        }

        void IAdvanced.UpsertNamedQuery<T>(string name, Action<IQueryBuilder<T>> spec)
        {
            Try(() =>
            {
                Ensure.That(name, "name").IsNotNullOrWhiteSpace();
                Ensure.That(spec, "spec").IsNotNull();

                var generator = Db.ProviderFactory.GetNamedQueryGenerator<T>(Db.StructureSchemas);
                TransactionalDbClient.UpsertSp(name, generator.Generate(name, spec));
            });
        }

        IEnumerable<T> IAdvanced.NamedQuery<T>(INamedQuery query)
        {
            return Try(() => OnNamedQuery<T>(query));
        }

        IEnumerable<T> IAdvanced.NamedQuery<T>(string name, Expression<Func<T, bool>> predicate)
        {
            return Try(() =>
            {
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
                queryBuilder.Where(predicate);
                var query = queryBuilder.Build();
                var sqlExpression = SqlExpressionBuilder.Process(query);

                var namedQuery = new NamedQuery(name);
                namedQuery.Add(sqlExpression.WhereCriteria.Parameters);

                return OnNamedQuery<T>(namedQuery);
            });
        }

        protected virtual IEnumerable<T> OnNamedQuery<T>(INamedQuery query) where T : class
        {
            Ensure.That(query, "query").IsNotNull();

            OnUpsertStructureSchema<T>();

            var sourceData = TransactionalDbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray());

            return Db.Serializer.DeserializeMany<T>(sourceData.ToArray());
        }

        IEnumerable<TOut> IAdvanced.NamedQueryAs<TContract, TOut>(INamedQuery query)
        {
            return Try(() => OnNamedQueryAs<TContract, TOut>(query));
        }

        IEnumerable<TOut> IAdvanced.NamedQueryAs<TContract, TOut>(string name, Expression<Func<TContract, bool>> predicate)
        {
            return Try(() =>
            {
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<TContract>(Db.StructureSchemas);
                queryBuilder.Where(predicate);
                var query = queryBuilder.Build();
                var sqlExpression = SqlExpressionBuilder.Process(query);

                var namedQuery = new NamedQuery(name);
                namedQuery.Add(sqlExpression.WhereCriteria.Parameters);

                return OnNamedQueryAs<TContract, TOut>(namedQuery);
            });
        }

        protected virtual IEnumerable<TOut> OnNamedQueryAs<TContract, TOut>(INamedQuery query)
            where TContract : class
            where TOut : class
        {
            Ensure.That(query, "query").IsNotNull();

            OnUpsertStructureSchema<TContract>();

            var sourceData = TransactionalDbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray());

            return Db.Serializer.DeserializeMany<TOut>(sourceData.ToArray());
        }

        IEnumerable<string> IAdvanced.NamedQueryAsJson<T>(INamedQuery query)
        {
            return Try(() => OnNamedQueryAsJson<T>(query));
        }

        IEnumerable<string> IAdvanced.NamedQueryAsJson<T>(string name, Expression<Func<T, bool>> predicate)
        {
            return Try(() =>
            {
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
                queryBuilder.Where(predicate);
                var query = queryBuilder.Build();
                var sqlExpression = SqlExpressionBuilder.Process(query);

                var namedQuery = new NamedQuery(name);
                namedQuery.Add(sqlExpression.WhereCriteria.Parameters);

                return OnNamedQueryAsJson<T>(namedQuery);
            });
        }

        protected virtual IEnumerable<string> OnNamedQueryAsJson<T>(INamedQuery query) where T : class
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

                var deleteIds = new List<IStructureId>(Db.Settings.MaxUpdateManyBatchSize);
                var keepQueue = new List<T>(Db.Settings.MaxUpdateManyBatchSize);
                var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
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
                    if (keepQueue.Count < Db.Settings.MaxUpdateManyBatchSize)
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

        public virtual ISisoQueryable<T> Query<T>() where T : class
        {
            return Try(() => new SisoQueryable<T>(Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas), QueryEngine));
        }

        bool IQueryEngine.Any<T>()
        {
            return Try(() => OnAny(TypeFor<T>.Type));
        }

        bool IQueryEngine.Any(Type structureType)
        {
            return Try(() => OnAny(structureType));
        }

        protected virtual bool OnAny(Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);

            return TransactionalDbClient.Any(structureSchema);
        }

        bool IQueryEngine.Any<T>(IQuery query)
        {
            return Try(() => OnAny(TypeFor<T>.Type, query));
        }

        bool IQueryEngine.Any(Type structureType, IQuery query)
        {
            return Try(() => OnAny(structureType, query));
        }

        protected virtual bool OnAny(Type structureType, IQuery query)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);

            if (!query.HasWhere)
                return TransactionalDbClient.Any(structureSchema);

            var whereSql = QueryGenerator.GenerateQueryReturningStrutureIds(query);
            return TransactionalDbClient.Any(structureSchema, whereSql);
        }

        int IQueryEngine.Count<T>()
        {
            return Try(() => OnCount(TypeFor<T>.Type));
        }

        int IQueryEngine.Count(Type structureType)
        {
            return Try(() => OnCount(structureType));
        }

        protected virtual int OnCount(Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);

            return TransactionalDbClient.RowCount(structureSchema);
        }

        int IQueryEngine.Count<T>(IQuery query)
        {
            return Try(() => OnCount(TypeFor<T>.Type, query));
        }

        int IQueryEngine.Count(Type structureType, IQuery query)
        {
            return Try(() => OnCount(structureType, query));
        }

        protected virtual int OnCount(Type structureType, IQuery query)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);

            if (!query.HasWhere)
                return TransactionalDbClient.RowCount(structureSchema);

            var whereSql = QueryGenerator.GenerateQueryReturningStrutureIds(query);
            return TransactionalDbClient.RowCountByQuery(structureSchema, whereSql);
        }

        public virtual bool Exists<T>(object id) where T : class 
        {
            return Try(() => OnExists(TypeFor<T>.Type, id));
        }

        public virtual bool Exists(Type structureType, object id)
        {
            return Try(() => OnExists(structureType, id));
        }

        protected bool OnExists(Type structureType, object id)
        {
            return Try(() =>
            {
                Ensure.That(structureType, "structureType").IsNotNull();
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = OnUpsertStructureSchema(structureType);

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return TransactionalDbClient.Exists(structureSchema, structureId);

                return Db.CacheProvider.Exists(
                    structureSchema,
                    structureId,
                    sid => TransactionalDbClient.Exists(structureSchema, sid));
            });
        }

        IEnumerable<T> IQueryEngine.Query<T>(IQuery query)
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

        IEnumerable<object> IQueryEngine.Query(IQuery query, Type structureType)
        {
            return Try(() =>
            {
                Ensure.That(structureType, "structureType").IsNotNull();
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = OnUpsertStructureSchema(structureType);

                IEnumerable<string> sourceData;

                if (query.IsEmpty)
                    sourceData = TransactionalDbClient.GetJsonOrderedByStructureId(structureSchema);
                else
                {
                    var sqlQuery = QueryGenerator.GenerateQuery(query);
                    sourceData = TransactionalDbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray());
                }

                return Db.Serializer.DeserializeMany(sourceData.ToArray(), structureType);
            });
        }

        IEnumerable<TResult> IQueryEngine.QueryAs<T, TResult>(IQuery query)
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

        IEnumerable<TResult> IQueryEngine.QueryAsAnonymous<T, TResult>(IQuery query, TResult template)
        {
            return Try(() => OnQueryAsAnonymous<T, TResult>(query, typeof(TResult)));
        }

        IEnumerable<TResult> IQueryEngine.QueryAsAnonymous<T, TResult>(IQuery query, Type templateType)
        {
            return Try(() => OnQueryAsAnonymous<T, TResult>(query, templateType));
        }

        protected virtual IEnumerable<TResult> OnQueryAsAnonymous<T, TResult>(IQuery query, Type templateType) where T :class where TResult : class 
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

            return Db.Serializer.DeserializeManyUsingTemplate<TResult>(sourceData.ToArray(), templateType);
        }

        IEnumerable<string> IQueryEngine.QueryAsJson<T>(IQuery query)
        {
            return Try(() => OnQueryAsJson(TypeFor<T>.Type, query));
        }

        IEnumerable<string> IQueryEngine.QueryAsJson(IQuery query, Type structuretype)
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
                    TransactionalDbClient.GetJsonById(sid, structureSchema),
                    structureSchema.Type.Type),
                CacheConsumeMode);

            return Db.Serializer.Serialize(item);
        }

        public virtual IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class
        {
            return Try(() => OnGetByIdsAsJson(TypeFor<T>.Type, ids));
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
                return TransactionalDbClient.GetJsonByIds(structureIds, structureSchema).Where(s => s != null).ToArray();

            var items = Db.CacheProvider.Consume(
                structureSchema,
                structureIds,
                sids => Db.Serializer.DeserializeMany(
                    TransactionalDbClient.GetJsonByIds(sids, structureSchema),
                    structureSchema.Type.Type).Where(s => s != null).ToArray(),
                CacheConsumeMode);

            return Db.Serializer.SerializeMany(items).ToArray();
        }

        public virtual ISession Insert<T>(T item) where T : class
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

            return this;
        }

        public virtual ISession InsertAs<T>(object item) where T : class
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

            return this;
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

            var item = Db.Serializer.Deserialize(json, structureType);
            var structureSchema = OnUpsertStructureSchema(structureType);

            var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, Db.ProviderFactory.GetIdentityStructureIdGenerator(OnCheckOutAndGetNextIdentity));
            var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
            var structure = structureBuilder.CreateStructure(item, structureSchema);
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

                var structureBuilder = Db.StructureBuilders.ForInserts(
                    structureSchema,
                    Db.ProviderFactory.GetIdentityStructureIdGenerator(OnCheckOutAndGetNextIdentity));

                var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                foreach (var structuresBatch in items.Batch(Db.Settings.MaxInsertManyBatchSize))
                    structureInserter.Insert(structureSchema, structureBuilder.CreateStructures(structuresBatch, structureSchema));
            });

            return this;
        }

        public virtual void InsertManyJson<T>(IEnumerable<string> json, Action<IEnumerable<string>> onBatchInserted = null) where T : class
        {
            Try(() => OnInsertManyJson(TypeFor<T>.Type, json, onBatchInserted));
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

            var structureBuilder = Db.StructureBuilders.ForInserts(
                structureSchema,
                Db.ProviderFactory.GetIdentityStructureIdGenerator(OnCheckOutAndGetNextIdentity));

            var structureInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
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
            Try(() => OnUpdate(TypeFor<T>.Type, item));

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

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

            var structureSchema = OnUpsertStructureSchema(structureType);
            var structureId = structureSchema.IdAccessor.GetValue(item);

            if (!structureSchema.HasConcurrencyToken)
            {
                var exists = TransactionalDbClient.Exists(structureSchema, structureId);
                if (!exists)
                    throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));
            }
            else
                OnEnsureConcurrencyTokenIsValid(structureSchema, structureId, item);

            Db.CacheProvider.NotifyDeleting(structureSchema, structureId);
            TransactionalDbClient.DeleteIndexesAndUniquesById(structureId, structureSchema);

            var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
            var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

            var bulkInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
            bulkInserter.Replace(structureSchema, updatedStructure);
        }

        public virtual ISession Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class
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
                    OnEnsureConcurrencyTokenIsValid(structureSchema, structureId, item);

                Db.CacheProvider.NotifyDeleting(structureSchema, structureId);
                TransactionalDbClient.DeleteIndexesAndUniquesById(structureId, structureSchema);

                var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
                var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

                var bulkInserter = Db.ProviderFactory.GetStructureInserter(TransactionalDbClient);
                bulkInserter.Replace(structureSchema, updatedStructure);
            });

            return this;
        }

        protected virtual void OnEnsureConcurrencyTokenIsValid(IStructureSchema structureSchema, IStructureId structureId, object newItem)
        {
            var existingJson = TransactionalDbClient.GetJsonById(structureId, structureSchema);

            if (string.IsNullOrWhiteSpace(existingJson))
                throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));

            var existingItem = Db.Serializer.Deserialize(existingJson, structureSchema.Type.Type);
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

        public virtual ISession DeleteById<T>(object id) where T : class
        {
            Try(() => OnDeleteById(TypeFor<T>.Type, id));

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

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

            var structureId = StructureId.ConvertFrom(id);
            var structureSchema = OnUpsertStructureSchema(structureType);

            Db.CacheProvider.NotifyDeleting(structureSchema, structureId);
            TransactionalDbClient.DeleteById(structureId, structureSchema);
        }

        public virtual ISession DeleteByIds<T>(params object[] ids) where T : class
        {
            Try(() => OnDeleteByIds(TypeFor<T>.Type, ids));

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

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;

            var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
            var structureSchema = OnUpsertStructureSchema(structureType);

            Db.CacheProvider.NotifyDeleting(structureSchema, structureIds);
            TransactionalDbClient.DeleteByIds(structureIds, structureSchema);
        }
    }
}