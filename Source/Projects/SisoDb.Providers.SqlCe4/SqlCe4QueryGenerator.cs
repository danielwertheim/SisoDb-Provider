using System.Collections.Generic;
using System.Linq;
using NCore;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Structures;

namespace SisoDb.SqlCe4
{
    public class SqlCe4QueryGenerator : DbQueryGenerator
    {
        protected override SqlQuery CreateSqlQuery(IQueryCommand queryCommand)
        {
            var sqlExpression = SqlExpressionBuilder.Process(queryCommand);

            var sql = string.Format(
                "select {0}min(s.[Json]) [Json]{1}{2} from [{3}] s inner join [{4}] si on si.[StructureId] = s.[StructureId]{5}{6}{7}{8}{9};",
                GenerateTakeString(queryCommand).AppendWith(" "),
                GenerateIncludesJsonOutputDefinitionString(sqlExpression).PrependWith(", "),
                GenerateColumnDefinitionSortingString(sqlExpression, "min({0})").PrependWith(", "),
                queryCommand.StructureSchema.GetStructureTableName(),
                queryCommand.StructureSchema.GetIndexesTableName(),
                GenerateMembersJoinString(queryCommand, sqlExpression).PrependWith(" "),
                GenerateIncluesJoinString(sqlExpression).PrependWith(" "),
                sqlExpression.WhereCriteria.CriteriaString.PrependWith(" where "),
                GenerateGroupingString(queryCommand).PrependWith(" group by "),
                GenerateSortingString(sqlExpression, "{0}").PrependWith(" order by "));

            return new SqlQuery(sql, sqlExpression.WhereCriteria.Parameters);
        }

        protected override SqlQuery CreateSqlQueryForPaging(IQueryCommand queryCommand)
        {
            var sqlExpression = SqlExpressionBuilder.Process(queryCommand);

            var offsetRows = (queryCommand.Paging.PageIndex * queryCommand.Paging.PageSize);
            var takeRows = queryCommand.TakeNumOfStructures;
            if (queryCommand.HasPaging)
                takeRows = queryCommand.Paging.PageSize;

            var sql = string.Format(
                "select min(s.[Json]) [Json]{0}{1} from [{2}] s inner join [{3}] si on si.[StructureId] = s.[StructureId]{4}{5}{6}{7} order by {8} offset @offsetRows rows{9};",
                GenerateIncludesJsonOutputDefinitionString(sqlExpression).PrependWith(", "),
                GenerateColumnDefinitionSortingString(sqlExpression, "min({0})").PrependWith(", "),
                queryCommand.StructureSchema.GetStructureTableName(),
                queryCommand.StructureSchema.GetIndexesTableName(),
                GenerateMembersJoinString(queryCommand, sqlExpression).PrependWith(" "),
                GenerateIncluesJoinString(sqlExpression).PrependWith(" "),
                sqlExpression.WhereCriteria.CriteriaString.PrependWith(" where "),
                GenerateGroupingString(queryCommand).PrependWith(" group by "),
                queryCommand.HasSortings ? GenerateSortingString(sqlExpression, "{0}") : "s.[StructureId]",
                GenerateFetchString(queryCommand).PrependWith(" "));

            var queryParams = new List<IDacParameter>(sqlExpression.WhereCriteria.Parameters)
            {
                new DacParameter("@offsetRows", offsetRows),
                new DacParameter("@takeRows", takeRows)
            };

            return new SqlQuery(sql, queryParams.ToArray());
        }

        protected virtual string GenerateFetchString(IQueryCommand queryCommand)
        {
            return queryCommand.HasTakeNumOfStructures || queryCommand.HasPaging
                ? "fetch next @takeRows rows only"
                : string.Empty;
        }

        protected virtual string GenerateColumnDefinitionSortingString(ISqlExpression sqlExpression, string decorateSortingWith)
        {
            var sortings = sqlExpression.SortingMembers.ToList();
            if (sortings.Count == 0)
                return string.Empty;

            var transformedSortings = new List<string>();

            foreach (var sorting in sortings)
            {
                transformedSortings.Add(string.Format("{0} {1}",
                    string.Format(decorateSortingWith, "{0}.[{1}]".Inject(sorting.Alias, sorting.IndexStorageColumnName)),
                    sorting.Alias));
            }

            return string.Join(", ", transformedSortings);
        }

        protected override string GenerateSortingString(ISqlExpression sqlExpression, string decorateSortingWith)
        {
            var sortings = sqlExpression.SortingMembers.ToList();
            if (sortings.Count == 0)
                return string.Empty;

            var transformedSortings = new List<string>();

            foreach (var sorting in sortings)
            {
                transformedSortings.Add(string.Format("{0} {1}",
                    string.Format(decorateSortingWith, sorting.Alias),
                    sorting.Direction));
            }

            return string.Join(", ", transformedSortings);
        }
    }
}