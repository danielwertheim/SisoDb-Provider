using System.Collections.Generic;
using System.Linq;
using NCore;
using SisoDb.Dac;
using SisoDb.Providers;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Structures;

namespace SisoDb.Sql2008
{
    public class Sql2008QueryGenerator : DbQueryGenerator
    {
        public Sql2008QueryGenerator(ISqlStatements sqlStatements) : base(sqlStatements) {}

        protected override SqlQuery CreateSqlQuery(IQueryCommand queryCommand)
        {
            var sqlExpression = SqlExpressionBuilder.Process(queryCommand);

            var formatter = new SqlQueryFormatter
            {
                Start = GenerateStartString(queryCommand, sqlExpression),
                End = GenerateEndString(queryCommand, sqlExpression),
                Take = GenerateTakeString(queryCommand),
                IncludedJsonMembers = GenerateIncludedJsonMembersString(sqlExpression),
                OrderByMembers = GenerateOrderByMembersString(queryCommand, sqlExpression),
                IncludedRowIds = GenerateIncludedRowIdsString(sqlExpression),
                MainStructureTable = queryCommand.StructureSchema.GetStructureTableName(),
                WhereAndSortingJoins = GenerateWhereAndSortingJoins(queryCommand, sqlExpression),
                MatchingIncludesJoins = GenerateMatchingIncludesJoins(queryCommand, sqlExpression),
                WhereCriteria = GenerateWhereCriteriaString(sqlExpression),
                IncludesJoins = GenerateIncludesJoins(sqlExpression),
                OrderBy = GenerateOrderByString(queryCommand, sqlExpression),
                Paging = GeneratePagingString(queryCommand, sqlExpression).PrependWith(", "),
            };

            IDacParameter[] parameters;

            if (queryCommand.HasPaging)
            {
                var takeFromRowNum = (queryCommand.Paging.PageIndex * queryCommand.Paging.PageSize) + 1;
                var takeToRowNum = (takeFromRowNum + queryCommand.Paging.PageSize) - 1;

                parameters = new List<IDacParameter>(sqlExpression.WhereCriteria.Parameters)
                {
                    new DacParameter("@pagingFrom", takeFromRowNum),
                    new DacParameter("@pagingTo", takeToRowNum)
                }.ToArray();
            }
            else
                parameters = sqlExpression.WhereCriteria.Parameters;

            return new SqlQuery(formatter.Format(SqlStatements.GetSql("Query")), parameters);
        }

        protected override SqlQuery CreateSqlQueryReturningStructureIds(IQueryCommand queryCommand)
        {
            var sqlExpression = SqlExpressionBuilder.Process(queryCommand);

            var formatter = new SqlQueryFormatter
            {
                MainStructureTable = queryCommand.StructureSchema.GetStructureTableName(),
                WhereAndSortingJoins = GenerateWhereAndSortingJoins(queryCommand, sqlExpression),
                WhereCriteria = GenerateWhereCriteriaString(sqlExpression)
            };

            return new SqlQuery(formatter.Format(SqlStatements.GetSql("QueryReturningStructureIds")), sqlExpression.WhereCriteria.Parameters);
        }

        protected override string GenerateOrderByMembersString(IQueryCommand queryCommand, ISqlExpression sqlExpression)
        {
            return queryCommand.HasPaging
                ? string.Empty
                : base.GenerateOrderByMembersString(queryCommand, sqlExpression);
        }

        protected override string GenerateOrderByString(IQueryCommand queryCommand, ISqlExpression sqlExpression)
        {
            return queryCommand.HasPaging
                ? string.Empty
                : base.GenerateOrderByString(queryCommand, sqlExpression);
        }

        protected override string GeneratePagingString(IQueryCommand queryCommand, ISqlExpression sqlExpression)
        {
            if(!queryCommand.HasPaging)
                return string.Empty;

            var s = string.Join(", ", sqlExpression.SortingMembers.Select(sorting => string.Format("min(mem{0}.[{1}]) {2}", sorting.Index, sorting.IndexStorageColumnName, sorting.Direction)));
            
            return string.Format("row_number() over (order by {0}) RowNum", s);
        }

        protected override string GenerateEndString(IQueryCommand queryCommand, ISqlExpression sqlExpression)
        {
            return queryCommand.HasPaging
                ? "where rs.RowNum between @pagingFrom and @pagingTo"
                : string.Empty;
        }
    }
}