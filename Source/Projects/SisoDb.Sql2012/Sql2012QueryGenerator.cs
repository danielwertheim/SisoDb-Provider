using System.Collections.Generic;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Sql;

namespace SisoDb.Sql2012
{
    public class Sql2012QueryGenerator : DbQueryGenerator
    {
        public Sql2012QueryGenerator(ISqlStatements sqlStatements) : base(sqlStatements) {}

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
				MainStructureTable = query.StructureSchema.GetStructureTableName(),
				WhereAndSortingJoins = GenerateWhereAndSortingJoins(query, sqlExpression),
                WhereCriteria = GenerateWhereCriteriaString(sqlExpression),
                IncludesJoins = GenerateIncludesJoins(query, sqlExpression),
				OrderBy = GenerateOrderByString(query, sqlExpression),
				Paging = GeneratePagingString(query, sqlExpression),
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

		//protected override string GenerateOrderByMembersString(IQuery query, ISqlExpression sqlExpression)
		//{
		//    return query.HasPaging
		//        ? string.Empty
		//        : base.GenerateOrderByMembersString(query, sqlExpression);
		//}

		//protected override string GenerateOrderByString(IQuery query, ISqlExpression sqlExpression)
		//{
		//    return query.HasPaging
		//        ? string.Empty
		//        : base.GenerateOrderByString(query, sqlExpression);
		//}

		protected override string GeneratePagingString(IQuery queryCommand, ISqlExpression sqlExpression)
		{
			return queryCommand.HasPaging
				? "offset @offsetRows rows fetch next @takeRows rows only"
				: string.Empty;
		}

		//protected override string GenerateEndString(IQuery query, ISqlExpression sqlExpression)
		//{
		//    return query.HasPaging
		//        ? "where rs.RowNum between @pagingFrom and @pagingTo"
		//        : string.Empty;
		//}
    }
}