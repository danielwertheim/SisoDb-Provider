using System.Collections.Generic;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Sql;

namespace SisoDb.SqlCe4
{
    public class SqlCe4QueryGenerator : DbQueryGenerator
    {
        public SqlCe4QueryGenerator(ISqlStatements sqlStatements) : base(sqlStatements) {}

		protected override DbQuery CreateSqlQuery(IQuery query)
        {
			var sqlExpression = SqlExpressionBuilder.Process(query);

            var formatter = new SqlQueryFormatter
            {
				Start = GenerateStartString(query, sqlExpression),
				End = GenerateEndString(query, sqlExpression),
				Take = GenerateTakeString(query),
                IncludedJsonMembers = GenerateIncludedJsonMembersString(sqlExpression),
				OrderByMembers = GenerateOrderByMembersString(query, sqlExpression),
                IncludedRowIds = GenerateIncludedRowIdsString(sqlExpression),
				MainStructureTable = query.StructureSchema.GetStructureTableName(),
				WhereAndSortingJoins = GenerateWhereAndSortingJoins(query, sqlExpression),
				MatchingIncludesJoins = GenerateMatchingIncludeJoins(query, sqlExpression),
                WhereCriteria = GenerateWhereCriteriaString(sqlExpression),
                IncludesJoins = GenerateIncludesJoins(sqlExpression),
				OrderBy = GenerateOrderByString(query, sqlExpression),
				Paging = GeneratePagingString(query, sqlExpression)
            };

            IDacParameter[] parameters;

			if (query.HasPaging)
            {
				var offsetRows = (query.Paging.PageIndex * query.Paging.PageSize);
				var takeRows = query.Paging.PageSize;

                parameters = new List<IDacParameter>(sqlExpression.WhereCriteria.Parameters)
                {
                    new DacParameter("@offsetRows", offsetRows),
                    new DacParameter("@takeRows", takeRows)
                }.ToArray();
            }
            else
                parameters = sqlExpression.WhereCriteria.Parameters;
            
            return new DbQuery(formatter.Format(SqlStatements.GetSql("Query")), parameters);
        }

		protected override DbQuery CreateSqlQueryReturningStructureIds(IQuery query)
        {
			var sqlExpression = SqlExpressionBuilder.Process(query);

            var formatter = new SqlQueryFormatter
            {
				MainStructureTable = query.StructureSchema.GetStructureTableName(),
				WhereAndSortingJoins = GenerateWhereAndSortingJoins(query, sqlExpression),
                WhereCriteria = GenerateWhereCriteriaString(sqlExpression)
            };

            return new DbQuery(formatter.Format(SqlStatements.GetSql("QueryReturningStructureIds")), sqlExpression.WhereCriteria.Parameters);
        }

		protected override string GeneratePagingString(IQuery queryCommand, ISqlExpression sqlExpression)
        {
            return queryCommand.HasPaging
                ? "offset @offsetRows rows fetch next @takeRows rows only"
                : string.Empty;
        }
    }
}