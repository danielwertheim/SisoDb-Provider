using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using NCore;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Structures;

namespace SisoDb.Querying
{
    public abstract class DbQueryGenerator : IDbQueryGenerator
    {
        protected readonly static string StructureIdColumnName = string.Format("[{0}]", StructureStorageSchema.Fields.Id.Name);

        protected readonly SqlExpressionBuilder SqlExpressionBuilder;

        protected DbQueryGenerator()
        {
            SqlExpressionBuilder = new SqlExpressionBuilder();
        }

        public SqlQuery GenerateQuery(IQueryCommand queryCommand)
        {
            Ensure.That(queryCommand, "queryCommand").IsNotNull();

            return queryCommand.HasPaging
                ? CreateSqlQueryForPaging(queryCommand)
                : CreateSqlQuery(queryCommand);
        }

        public SqlQuery GenerateQueryReturningStrutureIds(IQueryCommand queryCommand)
        {
            Ensure.That(queryCommand, "queryCommand").IsNotNull();

            if (!queryCommand.HasWhere || (queryCommand.HasIncludes || queryCommand.HasSortings || queryCommand.HasPaging))
                throw new ArgumentException(ExceptionMessages.DbQueryGenerator_GenerateWhere);

            return CreateSqlQueryReturningStructureIds(queryCommand);
        }

        protected virtual SqlQuery CreateSqlQueryReturningStructureIds(IQueryCommand queryCommand)
        {
            var sqlExpression = SqlExpressionBuilder.Process(queryCommand);

            var sql = string.Format(
                "select {0}s.[StructureId] from [{1}] s{2}{3} group by s.[StructureId]",
                GenerateTakeString(queryCommand).AppendWith(" "),
                queryCommand.StructureSchema.GetStructureTableName(),
                GenerateMembersJoinString(queryCommand, sqlExpression).PrependWith(" "),
                sqlExpression.WhereCriteria.CriteriaString.PrependWith(" where "));

            return new SqlQuery(sql, sqlExpression.WhereCriteria.Parameters);
        }

        protected virtual SqlQuery CreateSqlQuery(IQueryCommand queryCommand)
        {
            var sqlExpression = SqlExpressionBuilder.Process(queryCommand);

            var sql = string.Format(
                "select {0}min(s.[Json]) [Json]{1} from [{2}] s inner join [{3}] si on si.[StructureId] = s.[StructureId]{4}{5}{6}{7}{8};",
                GenerateTakeString(queryCommand).AppendWith(" "),
                GenerateIncludesJsonOutputDefinitionString(sqlExpression).PrependWith(", "),
                queryCommand.StructureSchema.GetStructureTableName(),
                queryCommand.StructureSchema.GetIndexesTableName(),
                GenerateMembersJoinString(queryCommand, sqlExpression).PrependWith(" "),
                GenerateIncluesJoinString(sqlExpression).PrependWith(" "),
                sqlExpression.WhereCriteria.CriteriaString.PrependWith(" where "),
                GenerateGroupingString(queryCommand).PrependWith(" group by "),
                GenerateSortingString(sqlExpression, "min({0})").PrependWith(" order by "));

            return new SqlQuery(sql, sqlExpression.WhereCriteria.Parameters);
        }

        protected virtual SqlQuery CreateSqlQueryForPaging(IQueryCommand queryCommand)
        {
            var sqlExpression = SqlExpressionBuilder.Process(queryCommand);

            var innerSelect = string.Format(
                "select min(s.[Json]) [Json]{0}, row_number() over (order by {1}) as RowNum from [{2}] s inner join [{3}] si on si.[StructureId] = s.[StructureId]{4}{5}{6}{7}",
                GenerateIncludesJsonOutputDefinitionString(sqlExpression).PrependWith(", "),
                queryCommand.HasSortings ? GenerateSortingString(sqlExpression, "min({0})") : "s.[StructureId]",
                queryCommand.StructureSchema.GetStructureTableName(),
                queryCommand.StructureSchema.GetIndexesTableName(),
                GenerateMembersJoinString(queryCommand, sqlExpression).PrependWith(" "),
                GenerateIncluesJoinString(sqlExpression).PrependWith(" "),
                sqlExpression.WhereCriteria.CriteriaString.PrependWith(" where "),
                GenerateGroupingString(queryCommand).PrependWith(" group by "));

            var sql = string.Format("with pagedRs as ({0}) select {1}pagedRs.[Json]{2} from pagedRs where pagedRs.[RowNum] between @pagingFrom and @pagingTo;",
                innerSelect,
                GenerateTakeString(queryCommand).AppendWith(" "),
                GenerateIncludesJsonOutputDefinitionString(sqlExpression).PrependWith(", "));

            var takeFromRowNum = (queryCommand.Paging.PageIndex * queryCommand.Paging.PageSize) + 1;
            var takeToRowNum = (takeFromRowNum + queryCommand.Paging.PageSize) - 1;
            var queryParams = new List<IDacParameter>(sqlExpression.WhereCriteria.Parameters)
            {
                new DacParameter("@pagingFrom", takeFromRowNum),
                new DacParameter("@pagingTo", takeToRowNum)
            };

            return new SqlQuery(sql, queryParams.ToArray());
        }

        protected virtual string GenerateTakeString(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasTakeNumOfStructures)
                return string.Empty;

            return string.Format("top({0})", queryCommand.TakeNumOfStructures);
        }

        protected virtual string GenerateGroupingString(IQueryCommand queryCommand)
        {
            var shouldHaveGrouping = queryCommand.HasIncludes || queryCommand.HasPaging || queryCommand.HasSortings || queryCommand.HasWhere;

            if (!shouldHaveGrouping)
                return string.Empty;

            return "s.[StructureId]";
        }

        protected virtual string GenerateSortingString(ISqlExpression sqlExpression, string decorateSortingWith)
        {
            var sortings = sqlExpression.SortingMembers.ToList();
            if (sortings.Count == 0)
                return string.Empty;

            var transformedSortings = new List<string>();

            foreach (var sorting in sortings)
            {
                transformedSortings.Add(string.Format("{0} {1}",
                    string.Format(decorateSortingWith, "{0}.[{1}]".Inject(sorting.Alias, sorting.IndexStorageColumnName)),
                    sorting.Direction));
            }

            return string.Join(", ", transformedSortings);
        }

        protected virtual string GenerateMembersJoinString(IQueryCommand queryCommand, ISqlExpression sqlExpression)
        {
            var wheres = sqlExpression.WhereMembers.ToList();
            var sortings = sqlExpression.SortingMembers.ToList();

            var indexesTableName = queryCommand.StructureSchema.GetIndexesTableName();

            var joins = new List<string>(wheres.Count + sortings.Count);

            const string joinFormat = "inner join [{0}] {1} on {1}.[StructureId] = s.[StructureId] and {1}.[MemberPath] = '{2}'";

            if (wheres.Count > 0)
            {
                joins.AddRange(wheres.Select(where =>
                    string.Format(joinFormat,
                    indexesTableName,
                    where.Alias,
                    where.MemberPath)));
            }

            if (sortings.Count > 0)
            {
                joins.AddRange(sortings.Select(sorting =>
                    string.Format(joinFormat,
                    indexesTableName,
                    sorting.Alias,
                    sorting.MemberPath)));
            }

            return string.Join(" ", joins.Distinct());
        }

        protected virtual string GenerateIncluesJoinString(ISqlExpression sqlExpression)
        {
            var includes = sqlExpression.Includes.ToList();

            return includes.Count == 0
                ? string.Empty
                : string.Join(" ", includes.Select(inc => inc.JoinString));
        }

        protected virtual string GenerateIncludesJsonOutputDefinitionString(ISqlExpression sqlExpression)
        {
            var includes = sqlExpression.Includes.ToList();

            if (includes.Count == 0)
                return string.Empty;

            return string.Join(", ", includes.Select(inc => inc.JsonOutputDefinition));
        }
    }
}