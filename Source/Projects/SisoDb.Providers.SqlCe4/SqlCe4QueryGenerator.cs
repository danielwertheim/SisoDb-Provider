using System.Collections.Generic;
using SisoDb.Dac;
using SisoDb.Providers;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Structures;

namespace SisoDb.SqlCe4
{
    public class SqlCe4QueryGenerator : DbQueryGenerator
    {
        public SqlCe4QueryGenerator(ISqlStatements sqlStatements) : base(sqlStatements) {}

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
                Paging = GeneratePagingString(queryCommand, sqlExpression)
            };

            IDacParameter[] parameters;

            if (queryCommand.HasPaging)
            {
                var offsetRows = (queryCommand.Paging.PageIndex * queryCommand.Paging.PageSize);
                var takeRows = queryCommand.Paging.PageSize;

                parameters = new List<IDacParameter>(sqlExpression.WhereCriteria.Parameters)
                {
                    new DacParameter("@offsetRows", offsetRows),
                    new DacParameter("@takeRows", takeRows)
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

        protected override string GeneratePagingString(IQueryCommand queryCommand, ISqlExpression sqlExpression)
        {
            return queryCommand.HasPaging
                ? "offset @offsetRows rows fetch next @takeRows rows only"
                : string.Empty;
        }
    }
}