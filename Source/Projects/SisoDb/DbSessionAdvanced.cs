using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public class DbSessionAdvanced: IAdvanced
    {
        protected readonly ISessionExecutionContext ExecutionContext;
        protected ISisoDatabase Db { get { return ExecutionContext.Session.Db; } }
        protected IDbClient DbClient { get { return ExecutionContext.Session.DbClient; } }
        protected readonly IDbQueryGenerator QueryGenerator;
        protected readonly ISqlExpressionBuilder SqlExpressionBuilder;
        
        public DbSessionAdvanced(ISessionExecutionContext executionContext, IDbQueryGenerator queryGenerator, ISqlExpressionBuilder sqlExpressionBuilder)
        {
            Ensure.That(queryGenerator, "queryGenerator").IsNotNull();
            Ensure.That(sqlExpressionBuilder, "sqlExpressionBuilder").IsNotNull();
            Ensure.That(executionContext, "executionContext").IsNotNull();

            ExecutionContext = executionContext;
            QueryGenerator = queryGenerator;
            SqlExpressionBuilder = sqlExpressionBuilder;
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

        public virtual void NonQuery(string sql, params IDacParameter[] parameters)
        {
            Try(() =>
            {
                Ensure.That(sql, "sql").IsNotNullOrWhiteSpace();
                DbClient.ExecuteNonQuery(sql, parameters);
            });
        }

        public virtual void UpsertNamedQuery<T>(string name, Action<IQueryBuilder<T>> spec) where T : class
        {
            Try(() =>
            {
                Ensure.That(name, "name").IsNotNullOrWhiteSpace();
                Ensure.That(spec, "spec").IsNotNull();

                var generator = Db.ProviderFactory.GetNamedQueryGenerator<T>(Db.StructureSchemas);
                DbClient.UpsertSp(name, generator.Generate(name, spec));
            });
        }

        public virtual IEnumerable<T> NamedQuery<T>(INamedQuery query) where T : class
        {
            return Try(() => OnNamedQueryAs<T, T>(query));
        }

        public virtual IEnumerable<T> NamedQuery<T>(string name, Expression<Func<T, bool>> predicate) where T : class
        {
            return Try(() =>
            {
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
                queryBuilder.Where(predicate);
                var query = queryBuilder.Build();
                var sqlExpression = SqlExpressionBuilder.Process(query);

                var namedQuery = new NamedQuery(name);
                namedQuery.Add(sqlExpression.WhereCriteria.Parameters);

                return OnNamedQueryAs<T, T>(namedQuery);
            });
        }

        public virtual IEnumerable<TOut> NamedQueryAs<TContract, TOut>(INamedQuery query) where TContract : class where TOut : class
        {
            return Try(() => OnNamedQueryAs<TContract, TOut>(query));
        }

        public virtual IEnumerable<TOut> NamedQueryAs<TContract, TOut>(string name, Expression<Func<TContract, bool>> predicate) where TContract : class where TOut : class
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

            var structureSchema = OnUpsertStructureSchema<TContract>();

            return Db.Serializer.DeserializeMany<TOut>(DbClient.YieldJsonBySp(structureSchema, query.Name, query.Parameters));
        }

        public virtual IEnumerable<string> NamedQueryAsJson<T>(INamedQuery query) where T : class
        {
            return Try(() => OnNamedQueryAsJson<T>(query));
        }

        public virtual IEnumerable<string> NamedQueryAsJson<T>(string name, Expression<Func<T, bool>> predicate) where T : class
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
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema<T>();

            return DbClient.YieldJsonBySp(structureSchema, query.Name, query.Parameters);
        }

        public virtual IEnumerable<T> RawQuery<T>(IRawQuery query) where T : class
        {
            return Try(() => OnRawQueryAs<T, T>(query));
        }

        public virtual IEnumerable<TOut> RawQueryAs<TContract, TOut>(IRawQuery query) where TContract : class where TOut : class
        {
            return Try(() => OnRawQueryAs<TContract, TOut>(query));
        }

        protected virtual IEnumerable<TOut> OnRawQueryAs<TContract, TOut>(IRawQuery query)
            where TContract : class
            where TOut : class
        {
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema<TContract>();

            return Db.Serializer.DeserializeMany<TOut>(DbClient.YieldJson(structureSchema, query.QueryString, query.Parameters));
        }

        public virtual IEnumerable<string> RawQueryAsJson<T>(IRawQuery query) where T : class
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = OnUpsertStructureSchema<T>();

                return DbClient.YieldJson(structureSchema, query.QueryString, query.Parameters);
            });
        }
    }
}