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
        protected ISisoDbTransaction Transaction;

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
            Transaction = SisoDbTransaction.CreateRequired();
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);

            if (Transaction == null)
                throw new ObjectDisposedException(ExceptionMessages.Session_AllreadyDisposed);

            Transaction.Dispose();
            Transaction = null;
        }

        protected virtual void WithDbClient(Action<IDbClient> action)
        {
            using (var dbClient = Db.ProviderFactory.GetDbClient(Db.ConnectionInfo))
            {
                action.Invoke(dbClient);
            }
        }

        protected virtual T WithDbClient<T>(Func<IDbClient, T> action)
        {
            using (var dbClient = Db.ProviderFactory.GetDbClient(Db.ConnectionInfo))
            {
                return action.Invoke(dbClient);
            }
        }

        //protected virtual void UpsertStructureSet(IStructureSchema structureSchema)
        //{
        //    using (var t = SisoDbTransaction.CreateSuppressed())
        //    {
        //        WithDbClient(dbClient => Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient));
        //    }
        //}

        //TODO: Yuck!
        protected virtual long CheckOutAndGetNextIdentity(IDbClient dbClient, IStructureSchema structureSchema, int numOfIds)
        {
            return dbClient.CheckOutAndGetNextIdentity(structureSchema.Name, numOfIds);
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

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    var sourceData = dbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray());
                    return Db.Serializer.DeserializeMany<T>(sourceData.ToArray());
                });
            });
        }

        IEnumerable<TOut> IAdvancedQueries.NamedQueryAs<TContract, TOut>(INamedQuery query)
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<TContract>();

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    var sourceData = dbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray());
                    return Db.Serializer.DeserializeMany<TOut>(sourceData.ToArray());
                });
            });
        }

        IEnumerable<string> IAdvancedQueries.NamedQueryAsJson<T>(INamedQuery query)
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    return dbClient.YieldJsonBySp(query.Name, query.Parameters.ToArray()).ToArray();
                });
            });
        }

        IEnumerable<T> IAdvancedQueries.RawQuery<T>(IRawQuery query)
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    var sourceData = dbClient.YieldJson(query.QueryString, query.Parameters.ToArray());
                    return Db.Serializer.DeserializeMany<T>(sourceData.ToArray());
                });
            });
        }

        IEnumerable<TOut> IAdvancedQueries.RawQueryAs<TContract, TOut>(IRawQuery query)
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<TContract>();

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    var sourceData = dbClient.YieldJson(query.QueryString, query.Parameters.ToArray());
                    return Db.Serializer.DeserializeMany<TOut>(sourceData.ToArray());
                });
            });
        }

        IEnumerable<string> IAdvancedQueries.RawQueryAsJson<T>(IRawQuery query)
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    return dbClient.YieldJson(query.QueryString, query.Parameters.ToArray()).ToArray();
                });
            });
        }

        public virtual int Count<T>(IQuery query) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();

                if (!query.HasWhere)
                    return WithDbClient(dbClient =>
                    {
                        Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                        return dbClient.RowCount(structureSchema);
                    });

                var whereSql = QueryGenerator.GenerateQueryReturningStrutureIds(query);

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    return dbClient.RowCountByQuery(structureSchema, whereSql);
                });
            });
        }

        public virtual bool Exists<T>(object id) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                var structureId = StructureId.ConvertFrom(id);

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return WithDbClient(dbClient =>
                    {
                        Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                        return dbClient.Exists(structureId, structureSchema);
                    });

                return Db.CacheProvider.Exists<T>(
                    structureSchema,
                    structureId,
                    sid => WithDbClient(dbClient =>
                    {
                        Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                        return dbClient.Exists(sid, structureSchema);
                    }));
            });
        }

        public virtual T GetById<T>(object id) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                var structureId = StructureId.ConvertFrom(id);

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);

                    if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                        return Db.Serializer.Deserialize<T>(dbClient.GetJsonById(structureId, structureSchema));

                    return Db.CacheProvider.Consume(
                        structureSchema,
                        structureId,
                        sid => Db.Serializer.Deserialize<T>(dbClient.GetJsonById(structureId, structureSchema)),
                        CacheConsumeModeWhenReading);
                });
            });
        }

        public virtual IEnumerable<T> GetByIds<T>(params object[] ids) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);

                    if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                        Db.Serializer.DeserializeMany<T>(dbClient.GetJsonByIds(structureIds, structureSchema).ToArray());

                    return Db.CacheProvider.Consume(
                        structureSchema,
                        structureIds,
                        sids => Db.Serializer.DeserializeMany<T>(dbClient.GetJsonByIds(structureIds, structureSchema).ToArray()),
                        CacheConsumeModeWhenReading);
                });
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

                if (!structureSchema.IdAccessor.IdType.IsIdentity())
                    throw new SisoDbException(
                        ExceptionMessages.SisoDbNotSupportedByProviderException.Inject(Db.ProviderFactory.ProviderType, ExceptionMessages.ReadSession_GetByIdInterval_WrongIdType));

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    return Db.Serializer.DeserializeMany<T>(dbClient.GetJsonWhereIdIsBetween(structureIdFrom, structureIdTo, structureSchema).ToArray());
                });
            });
        }

        public virtual TOut GetByIdAs<TContract, TOut>(object id) where TContract : class where TOut : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<TContract>();
                var structureId = StructureId.ConvertFrom(id);

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);

                    if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                        return Db.Serializer.Deserialize<TOut>(dbClient.GetJsonById(structureId, structureSchema));

                    return Db.CacheProvider.Consume(
                        structureSchema,
                        structureId,
                        sid => Db.Serializer.Deserialize<TOut>(dbClient.GetJsonById(structureId, structureSchema)),
                        CacheConsumeModeWhenReading);
                });
            });
        }

        public virtual IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids) where TContract : class where TOut : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureSchema = Db.StructureSchemas.GetSchema<TContract>();
                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);

                    if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                        return Db.Serializer.DeserializeMany<TOut>(dbClient.GetJsonByIds(structureIds, structureSchema).ToArray());

                    return Db.CacheProvider.Consume(
                        structureSchema,
                        structureIds,
                        sids => Db.Serializer.DeserializeMany<TOut>(dbClient.GetJsonByIds(structureIds, structureSchema).ToArray()),
                        CacheConsumeModeWhenReading);
                });
            });
        }

        public virtual string GetByIdAsJson<T>(object id) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                var structureId = StructureId.ConvertFrom(id);

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    return dbClient.GetJsonById(structureId, structureSchema);
                });
            });
        }

        public virtual IEnumerable<string> GetByIdsAsJson<T>(params object[] ids) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    return dbClient.GetJsonByIds(structureIds, structureSchema).ToArray();
                });
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

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);

                    IEnumerable<string> sourceData;

                    if (query.IsEmpty)
                        sourceData = dbClient.GetJsonOrderedByStructureId(structureSchema);
                    else
                    {
                        var sqlQuery = QueryGenerator.GenerateQuery(query);
                        sourceData = dbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray());
                    }

                    return Db.Serializer.DeserializeMany<T>(sourceData.ToArray());
                });
            });
        }

        public virtual IEnumerable<TResult> QueryAs<T, TResult>(IQuery query) where T : class where TResult : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);

                    IEnumerable<string> sourceData;

                    if (query.IsEmpty)
                        sourceData = dbClient.GetJsonOrderedByStructureId(structureSchema);
                    else
                    {
                        var sqlQuery = QueryGenerator.GenerateQuery(query);
                        sourceData = dbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray());
                    }
                    
                    return Db.Serializer.DeserializeMany<TResult>(sourceData.ToArray());
                });
            });
        }

        public virtual IEnumerable<TResult> QueryAsAnonymous<T, TResult>(IQuery query, TResult template) where T : class where TResult : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);

                    IEnumerable<string> sourceData;

                    if (query.IsEmpty)
                        sourceData = dbClient.GetJsonOrderedByStructureId(structureSchema);
                    else
                    {
                        var sqlQuery = QueryGenerator.GenerateQuery(query);
                        sourceData = dbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray());
                    }
                    
                    return Db.Serializer.DeserializeManyAnonymous(sourceData.ToArray(), template);
                });
            });
        }

        public virtual IEnumerable<string> QueryAsJson<T>(IQuery query) where T : class
        {
            return Transaction.Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();

                return WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);

                    if(query.IsEmpty)
                        return dbClient.GetJsonOrderedByStructureId(structureSchema).ToArray();

                    var sqlQuery = QueryGenerator.GenerateQuery(query);

                    return dbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray()).ToArray();
                });
            });
        }

        public virtual void Insert<T>(T item) where T : class
        {
            Transaction.Try(() =>
            {
                var structureSchema = Db.StructureSchemas.GetSchema<T>();

                WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, Db.ProviderFactory.GetIdentityStructureIdGenerator((s, n) => CheckOutAndGetNextIdentity(dbClient, s, n)));
                    var bulkInserter = Db.ProviderFactory.GetStructureInserter(dbClient);
                    bulkInserter.Insert(structureSchema, new[] { structureBuilder.CreateStructure(item, structureSchema) });
                });
            });
        }

        public virtual void InsertJson<T>(string json) where T : class
        {
            Transaction.Try(() =>
            {
                var item = Db.Serializer.Deserialize<T>(json);
                var structureSchema = Db.StructureSchemas.GetSchema<T>();

                WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    var structureBuilder = Db.StructureBuilders.ForInserts(structureSchema, Db.ProviderFactory.GetIdentityStructureIdGenerator((s, n) => CheckOutAndGetNextIdentity(dbClient, s, n)));
                    var bulkInserter = Db.ProviderFactory.GetStructureInserter(dbClient);
                    bulkInserter.Insert(structureSchema, new[] { structureBuilder.CreateStructure(item, structureSchema) });
                });
            });
        }

        public virtual void InsertMany<T>(IEnumerable<T> items) where T : class
        {
            Transaction.Try(() =>
            {
                var structureSchema = Db.StructureSchemas.GetSchema<T>();

                WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    var structureBuilder = Db.StructureBuilders.ForInserts(
                        structureSchema,
                        Db.ProviderFactory.GetIdentityStructureIdGenerator((s, n) => CheckOutAndGetNextIdentity(dbClient, s, n)));

                    var structureInserter = Db.ProviderFactory.GetStructureInserter(dbClient);
                    foreach (var structuresBatch in items.Batch(MaxInsertManyBatchSize))
                        structureInserter.Insert(structureSchema, structureBuilder.CreateStructures(structuresBatch, structureSchema));
                });
            });
        }

        public virtual void InsertManyJson<T>(IEnumerable<string> json) where T : class
        {
            Transaction.Try(() =>
            {
                var items = Db.Serializer.DeserializeMany<T>(json).ToArray(); //TODO: Hmm... Enumerable instead?
                var structureSchema = Db.StructureSchemas.GetSchema<T>();

                WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    var structureBuilder = Db.StructureBuilders.ForInserts(
                        structureSchema,
                        Db.ProviderFactory.GetIdentityStructureIdGenerator((s, n) => CheckOutAndGetNextIdentity(dbClient, s, n)));

                    var structureInserter = Db.ProviderFactory.GetStructureInserter(dbClient);
                    foreach (var structuresBatch in items.Batch(MaxInsertManyBatchSize))
                        structureInserter.Insert(structureSchema, structureBuilder.CreateStructures(structuresBatch, structureSchema));
                });
            });
        }

        public virtual void Update<T>(T item) where T : class
        {
            Transaction.Try(() =>
            {
                Ensure.That(item, "item").IsNotNull();
                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);
                var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

                WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    var existingItem = dbClient.GetJsonById(updatedStructure.Id, structureSchema);

                    if (string.IsNullOrWhiteSpace(existingItem))
                        throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(
                            updatedStructure.Name, updatedStructure.Id.Value));

                    Db.CacheProvider.NotifyDeleting(structureSchema, updatedStructure.Id);
                    dbClient.DeleteById(updatedStructure.Id, structureSchema);

                    var bulkInserter = Db.ProviderFactory.GetStructureInserter(dbClient);
                    bulkInserter.Insert(structureSchema, new[] { updatedStructure });
                });
            });
        }

        public virtual void UpdateMany<T>(Expression<Func<T, bool>> expression, Action<T> modifier) where T : class
        {
            Transaction.Try(() =>
            {
                Ensure.That(expression, "expression").IsNotNull();
                Ensure.That(modifier, "modifier").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                var deleteIds = new List<IStructureId>(MaxUpdateManyBatchSize);
                var keepQueue = new List<T>(MaxUpdateManyBatchSize);
                var structureBuilder = Db.StructureBuilders.ForUpdates(structureSchema);

                WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);

                    var structureInserter = Db.ProviderFactory.GetStructureInserter(dbClient);
                    var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(StructureSchemas);
                    var query = queryBuilder.Where(expression).Build();
                    var sqlQuery = QueryGenerator.GenerateQuery(query);
                    foreach (var structure in Db.Serializer.DeserializeMany<T>(
                        dbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray())))
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
                        dbClient.DeleteByIds(deleteIds, structureSchema);
                        deleteIds.Clear();

                        structureInserter.Insert(structureSchema,
                                                 structureBuilder.CreateStructures(keepQueue.ToArray(), structureSchema));
                        keepQueue.Clear();
                    }

                    if (keepQueue.Count > 0)
                    {
                        Db.CacheProvider.NotifyDeleting(structureSchema, deleteIds);
                        dbClient.DeleteByIds(deleteIds, structureSchema);
                        deleteIds.Clear();

                        structureInserter.Insert(structureSchema,
                                                 structureBuilder.CreateStructures(keepQueue.ToArray(), structureSchema));
                        keepQueue.Clear();
                    }
                });
            });
        }

        public virtual void DeleteById<T>(object id) where T : class
        {
            Transaction.Try(() =>
            {
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = Db.StructureSchemas.GetSchema<T>();

                Db.CacheProvider.NotifyDeleting(structureSchema, structureId);

                WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    dbClient.DeleteById(structureId, structureSchema);
                });
            });
        }

        public virtual void DeleteByIds<T>(params object[] ids) where T : class
        {
            Transaction.Try(() =>
            {
                Ensure.That(ids, "ids").HasItems();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
                Db.CacheProvider.NotifyDeleting(structureSchema, structureIds);

                WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    dbClient.DeleteByIds(structureIds, structureSchema);
                });
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

                if (!structureSchema.IdAccessor.IdType.IsIdentity())
                    throw new SisoDbException(
                        ExceptionMessages.SisoDbNotSupportedByProviderException.Inject(
                        Db.ProviderFactory.ProviderType, ExceptionMessages.WriteSession_DeleteByIdInterval_WrongIdType));

                if (Db.CacheProvider.IsEnabledFor(structureSchema))
                    throw new SisoDbException(
                        ExceptionMessages.WriteSession_DeleteByIdIntervalAndCachingNotSupported.Inject(structureSchema.Name));

                WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    dbClient.DeleteWhereIdIsBetween(structureIdFrom, structureIdTo, structureSchema);
                });
            });
        }

        public virtual void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
        {
            Transaction.Try(() =>
            {
                Ensure.That(expression, "expression").IsNotNull();

                var structureSchema = Db.StructureSchemas.GetSchema<T>();
                if (Db.CacheProvider.IsEnabledFor(structureSchema))
                    throw new SisoDbException(
                        ExceptionMessages.WriteSession_DeleteByQueryAndCachingNotSupported.Inject(structureSchema.Name));

                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(StructureSchemas);
                queryBuilder.Where(expression);

                var sql = QueryGenerator.GenerateQueryReturningStrutureIds(queryBuilder.Build());
                WithDbClient(dbClient =>
                {
                    Db.SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                    dbClient.DeleteByQuery(sql, structureSchema);
                });
            });
        }
    }
}