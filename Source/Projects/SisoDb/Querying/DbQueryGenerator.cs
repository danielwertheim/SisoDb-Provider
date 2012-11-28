using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;
using SisoDb.Querying.Sql;
using SisoDb.Resources;

namespace SisoDb.Querying
{
    public abstract class DbQueryGenerator : IDbQueryGenerator
    {
        protected readonly ISqlStatements SqlStatements;
        protected readonly ISqlExpressionBuilder SqlExpressionBuilder;

        protected DbQueryGenerator(ISqlStatements sqlStatements, ISqlExpressionBuilder sqlExpressionBuilder)
        {
            Ensure.That(sqlStatements, "sqlStatements").IsNotNull();
            Ensure.That(sqlExpressionBuilder, "sqlExpressionBuilder").IsNotNull();

            SqlStatements = sqlStatements;
            SqlExpressionBuilder = sqlExpressionBuilder;
        }

        public virtual IDbQuery GenerateQuery(IQuery query)
        {
            EnsureValidQuery(query);

            return CreateSqlQuery(query);
        }

        protected virtual IDbQuery CreateSqlQuery(IQuery query)
        {
            var sqlExpression = SqlExpressionBuilder.Process(query);
            var formatter = CreateSqlQueryFormatter(query, sqlExpression);
            var parameters = GenerateParameters(query, sqlExpression);

            if (query.HasNoDependencies())
                return new DbQuery(formatter.Format(SqlStatements.GetSql("QueryWithoutDependencies")), parameters, query.IsCacheable);

            if (!query.HasSortings && !query.HasPaging && !query.HasSkipNumOfStructures)
                return new DbQuery(formatter.Format(SqlStatements.GetSql("QueryWithoutPagingAndSorting")), parameters, query.IsCacheable);

            return new DbQuery(formatter.Format(SqlStatements.GetSql("Query")), parameters, query.IsCacheable);
        }

        public virtual IDbQuery GenerateQueryReturningStrutureIds(IQuery query)
        {
            EnsureQueryContainsOnlyWhereExpression(query);

            return CreateSqlQueryReturningStructureIds(query);
        }

        public virtual IDbQuery GenerateQueryReturningCountOfStrutureIds(IQuery query)
        {
            EnsureQueryContainsOnlyWhereExpression(query);

            return CreateSqlQueryReturningCountOfStructureIds(query);
        }

        protected virtual void EnsureValidQuery(IQuery query)
        {
            Ensure.That(query, "query").IsNotNull();
            if ((query.HasPaging || query.HasSkipNumOfStructures) && !query.HasSortings)
                throw new SisoDbException(ExceptionMessages.PagingMissesOrderBy);
        }

        protected virtual void EnsureQueryContainsOnlyWhereExpression(IQuery query)
        {
            Ensure.That(query, "query").IsNotNull();
            if (!query.HasWhere)
                throw new ArgumentException(ExceptionMessages.DbQueryGenerator_MissingWhere);

            if (query.HasSkipNumOfStructures || query.HasTakeNumOfStructures || query.HasSortings || query.HasPaging)
                throw new ArgumentException(ExceptionMessages.DbQueryGenerator_OnlyWhereExpressionsAreAllowed);
        }

        protected virtual IDbQuery CreateSqlQueryReturningStructureIds(IQuery query)
        {
            var sqlExpression = SqlExpressionBuilder.Process(query);
            var formatter = new SqlQueryFormatter
            {
                MainStructureTable = query.StructureSchema.GetStructureTableName(),
                WhereAndSortingJoins = GenerateWhereAndSortingJoins(query, sqlExpression),
                WhereCriteria = GenerateWhereCriteriaString(sqlExpression)
            };
            var parameters = GenerateParameters(query, sqlExpression);

            return new DbQuery(formatter.Format(SqlStatements.GetSql("QueryReturningStructureIds")), parameters, query.IsCacheable);
        }

        protected virtual IDbQuery CreateSqlQueryReturningCountOfStructureIds(IQuery query)
        {
            var sqlExpression = SqlExpressionBuilder.Process(query);
            var formatter = new SqlQueryFormatter
            {
                MainStructureTable = query.StructureSchema.GetStructureTableName(),
                WhereAndSortingJoins = GenerateWhereAndSortingJoins(query, sqlExpression),
                WhereCriteria = GenerateWhereCriteriaString(sqlExpression)
            };
            var parameters = GenerateParameters(query, sqlExpression);

            return new DbQuery(formatter.Format(SqlStatements.GetSql("QueryReturningCountOfStructureIds")), parameters, query.IsCacheable);
        }

        protected virtual SqlQueryFormatter CreateSqlQueryFormatter(IQuery query, ISqlExpression sqlExpression)
        {
            return new SqlQueryFormatter
            {
                Start = GenerateStartString(query, sqlExpression),
                End = GenerateEndString(query, sqlExpression),
                Take = GenerateTakeString(query),
                OrderByMembers = GenerateOrderByMembersString(query, sqlExpression),
                MainStructureTable = query.StructureSchema.GetStructureTableName(),
                WhereAndSortingJoins = GenerateWhereAndSortingJoins(query, sqlExpression),
                WhereCriteria = GenerateWhereCriteriaString(sqlExpression),
                OrderBy = GenerateOrderByString(query, sqlExpression),
                Paging = GeneratePagingString(query, sqlExpression)
            };
        }

        protected virtual IDacParameter[] GenerateParameters(IQuery query, ISqlExpression sqlExpression)
        {
            if (!query.HasWhere && !query.HasPaging && !query.HasSkipNumOfStructures)
                return new IDacParameter[0];

            var whereParams = sqlExpression.WhereCriteria.Parameters;
            var pagingParams = GenerateRsSizeLimitingParameters(query, sqlExpression);

            var allParams = new List<IDacParameter>(whereParams.Length + pagingParams.Length);

            if (whereParams.Any())
                allParams.AddRange(whereParams);

            if (pagingParams.Any())
                allParams.AddRange(pagingParams);

            return allParams.ToArray();
        }

        protected virtual string GenerateStartString(IQuery query, ISqlExpression sqlExpression)
        {
            return string.Empty;
        }

        protected virtual string GenerateEndString(IQuery query, ISqlExpression sqlExpression)
        {
            return string.Empty;
        }

        protected virtual string GenerateTakeString(IQuery query)
        {
            if (!query.HasTakeNumOfStructures)
                return string.Empty;

            if (query.HasPaging || query.HasSkipNumOfStructures)
                return string.Empty;

            return string.Format("top ({0})", query.TakeNumOfStructures);
        }

        protected virtual IDacParameter[] GenerateRsSizeLimitingParameters(IQuery query, ISqlExpression sqlExpression)
        {
            if (!query.HasPaging && !query.HasSkipNumOfStructures)
                return new IDacParameter[0];

            return query.HasPaging
                ? GenerateRsSizeLimitingParametersForPaging(query, sqlExpression)
                : GenerateRsSizeLimitingParametersForSkipAndTake(query, sqlExpression);
        }

        protected virtual IDacParameter[] GenerateRsSizeLimitingParametersForPaging(IQuery query, ISqlExpression sqlExpression)
        {
            return new IDacParameter[] 
            {
                new DacParameter("skipRows", (query.Paging.PageIndex * query.Paging.PageSize)),
                new DacParameter("takeRows", query.Paging.PageSize)
            };
        }

        protected virtual IDacParameter[] GenerateRsSizeLimitingParametersForSkipAndTake(IQuery query, ISqlExpression sqlExpression)
        {
            var ps = new List<IDacParameter>(2);

            if (query.SkipNumOfStructures.HasValue)
                ps.Add(new DacParameter("skipRows", query.SkipNumOfStructures.Value));

            if (query.TakeNumOfStructures.HasValue)
                ps.Add(new DacParameter("takeRows", query.TakeNumOfStructures.Value));

            return ps.ToArray();
        }

        protected virtual string GeneratePagingString(IQuery query, ISqlExpression sqlExpression)
        {
            if (!query.HasPaging && !query.HasSkipNumOfStructures)
                return string.Empty;

            if (query.HasPaging || (query.HasSkipNumOfStructures && query.HasTakeNumOfStructures))
                return "offset @skipRows rows fetch next @takeRows rows only";

            return "offset @skipRows rows";
        }

        protected virtual string GenerateOrderByMembersString(IQuery queryCommand, ISqlExpression sqlExpression)
        {
            var sortings = sqlExpression.SortingMembers.Select(
                sorting => (sorting.MemberPath != IndexStorageSchema.Fields.StructureId.Name)
                    ? string.Format("min(mem{0}.[{1}]) mem{0}", sorting.Index, sorting.IndexStorageColumnName)
                    : string.Empty).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            return sortings.Length == 0
                ? string.Empty
                : string.Join(", ", sortings);
        }

        protected virtual string GenerateOrderByString(IQuery query, ISqlExpression sqlExpression)
        {
            var sortings = sqlExpression.SortingMembers.Select(
                sorting => (sorting.MemberPath != IndexStorageSchema.Fields.StructureId.Name)
                            ? string.Format("mem{0} {1}", sorting.Index, sorting.Direction)
                            : string.Format("s.[{0}] {1}", StructureStorageSchema.Fields.Id.Name, sorting.Direction)).ToArray();

            return sortings.Length == 0
                ? string.Empty
                : string.Join(", ", sortings);
        }

        protected virtual string GenerateWhereAndSortingJoins(IQuery query, ISqlExpression sqlExpression)
        {
            var wheres = sqlExpression.WhereMembers.ToList();
            var sortings = sqlExpression.SortingMembers.ToList();

            var indexesTableNames = query.StructureSchema.GetIndexesTableNames();

            var joins = new List<string>(wheres.Count + sortings.Count);

            const string joinFormat = "left join [{0}] mem{1} on mem{1}.[StructureId] = s.[StructureId] and mem{1}.[MemberPath] = '{2}'";

            if (wheres.Count > 0)
            {
                joins.AddRange(wheres.Select(where =>
                    string.Format(joinFormat,
                    indexesTableNames.GetNameByType(where.DataTypeCode),
                    where.Index,
                    where.MemberPath)));
            }

            if (sortings.Count > 0)
            {
                joins.AddRange(sortings.Select(sorting =>
                    string.Format(joinFormat,
                    indexesTableNames.GetNameByType(sorting.DataTypeCode),
                    sorting.Index,
                    sorting.MemberPath)));
            }

            return string.Join(" ", joins.Distinct());
        }

        protected virtual string GenerateWhereCriteriaString(ISqlExpression sqlExpression)
        {
            return sqlExpression.WhereCriteria.CriteriaString;
        }
    }
}