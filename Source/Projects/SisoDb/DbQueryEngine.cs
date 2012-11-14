using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.Caching;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.Querying;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public class DbQueryEngine : IQueryEngine
    {
        protected readonly ISessionExecutionContext ExecutionContext;
        protected ISisoDatabase Db { get { return ExecutionContext.Session.Db; } }
        protected IDbClient DbClient { get { return ExecutionContext.Session.DbClient; } }
        protected readonly IDbQueryGenerator QueryGenerator;

        public DbQueryEngine(ISessionExecutionContext executionContext, IDbQueryGenerator queryGenerator)
        {
            Ensure.That(queryGenerator, "queryGenerator").IsNotNull();
            Ensure.That(executionContext, "executionContext").IsNotNull();

            ExecutionContext = executionContext;
            QueryGenerator = queryGenerator;
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

        public virtual bool Any<T>() where T : class
        {
            return Try(() => OnAny(typeof(T)));
        }

        public virtual bool Any(Type structureType)
        {
            return Try(() => OnAny(structureType));
        }

        protected virtual bool OnAny(Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);

            return DbClient.Any(structureSchema);
        }

        public virtual bool Any<T>(IQuery query) where T : class
        {
            return Try(() => OnAny(typeof(T), query));
        }

        public virtual bool Any(Type structureType, IQuery query)
        {
            return Try(() => OnAny(structureType, query));
        }

        protected virtual bool OnAny(Type structureType, IQuery query)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);

            if (!query.HasWhere)
                return DbClient.Any(structureSchema);

            var whereSql = QueryGenerator.GenerateQueryReturningCountOfStrutureIds(query);
            return DbClient.Any(structureSchema, whereSql);
        }

        public virtual int Count<T>() where T : class
        {
            return Try(() => OnCount(typeof(T)));
        }

        public virtual int Count(Type structureType)
        {
            return Try(() => OnCount(structureType));
        }

        protected virtual int OnCount(Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);

            return DbClient.RowCount(structureSchema);
        }

        public virtual int Count<T>(IQuery query) where T : class
        {
            return Try(() => OnCount(typeof(T), query));
        }

        public virtual int Count(Type structureType, IQuery query)
        {
            return Try(() => OnCount(structureType, query));
        }

        protected virtual int OnCount(Type structureType, IQuery query)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);

            if (!query.HasWhere)
                return DbClient.RowCount(structureSchema);

            var whereSql = QueryGenerator.GenerateQueryReturningCountOfStrutureIds(query);
            return DbClient.RowCountByQuery(structureSchema, whereSql);
        }

        public virtual bool Exists<T>(object id) where T : class
        {
            return Try(() => OnExists(typeof(T), id));
        }

        public virtual bool Exists(Type structureType, object id)
        {
            return Try(() => OnExists(structureType, id));
        }

        protected virtual bool OnExists(Type structureType, object id)
        {
            return Try(() =>
            {
                Ensure.That(structureType, "structureType").IsNotNull();
                Ensure.That(id, "id").IsNotNull();

                var structureId = StructureId.ConvertFrom(id);
                var structureSchema = OnUpsertStructureSchema(structureType);

                if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                    return DbClient.Exists(structureSchema, structureId);

                return Db.CacheProvider.Exists(
                    structureSchema,
                    structureId,
                    sid => DbClient.Exists(structureSchema, sid));
            });
        }

        public virtual IEnumerable<T> Query<T>(IQuery query) where T : class
        {
            return Try(() => OnQueryAs<T, T>(query));
        }

        public virtual IEnumerable<object> Query(IQuery query, Type structureType)
        {
            return Try(() => OnQuery(query, structureType));
        }

        protected virtual IEnumerable<object> OnQuery(IQuery query, Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);

            IEnumerable<string> sourceData;

            if (query.IsEmpty)
                sourceData = DbClient.GetJsonOrderedByStructureId(structureSchema);
            else
            {
                var sqlQuery = QueryGenerator.GenerateQuery(query);
                sourceData = DbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray());
            }

            return Db.Serializer.DeserializeMany(sourceData.ToArray(), structureType);
        }

        public virtual IEnumerable<TResult> QueryAs<T, TResult>(IQuery query)
            where T : class
            where TResult : class
        {
            return Try(() => OnQueryAs<T, TResult>(query));
        }

        protected virtual IEnumerable<TResult> OnQueryAs<T, TResult>(IQuery query)
            where T : class
            where TResult : class
        {
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema<T>();

            IEnumerable<string> sourceData;

            if (query.IsEmpty)
                sourceData = DbClient.GetJsonOrderedByStructureId(structureSchema);
            else
            {
                var sqlQuery = QueryGenerator.GenerateQuery(query);
                sourceData = DbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray());
            }

            return Db.Serializer.DeserializeMany<TResult>(sourceData.ToArray());
        }

        public virtual IEnumerable<string> QueryAsJson<T>(IQuery query) where T : class
        {
            return Try(() => OnQueryAsJson(typeof(T), query));
        }

        public virtual IEnumerable<string> QueryAsJson(IQuery query, Type structuretype)
        {
            return Try(() => OnQueryAsJson(structuretype, query));
        }

        protected virtual IEnumerable<string> OnQueryAsJson(Type structuretype, IQuery query)
        {
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structuretype);

            if (query.IsEmpty)
                return DbClient.GetJsonOrderedByStructureId(structureSchema).ToArray();

            var sqlQuery = QueryGenerator.GenerateQuery(query);

            return DbClient.YieldJson(sqlQuery.Sql, sqlQuery.Parameters.ToArray()).ToArray();
        }
    }
}