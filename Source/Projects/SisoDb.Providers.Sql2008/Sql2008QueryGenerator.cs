using System.Collections.Generic;
using System.Linq;
using NCore;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Structures;

namespace SisoDb.Sql2008
{
    public class Sql2008QueryGenerator : DbQueryGenerator
    {
        public Sql2008QueryGenerator(ISqlStatements sqlStatements) : base(sqlStatements) {}

		protected override SqlQuery CreateSqlQuery(IQuery query)
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
				MatchingIncludesJoins = GenerateMatchingIncludesJoins(query, sqlExpression),
                WhereCriteria = GenerateWhereCriteriaString(sqlExpression),
                IncludesJoins = GenerateIncludesJoins(sqlExpression),
				OrderBy = GenerateOrderByString(query, sqlExpression),
				Paging = GeneratePagingString(query, sqlExpression).PrependWith(", "),
            };

            IDacParameter[] parameters;

			if (query.HasPaging)
            {
				var takeFromRowNum = (query.Paging.PageIndex * query.Paging.PageSize) + 1;
				var takeToRowNum = (takeFromRowNum + query.Paging.PageSize) - 1;

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

		protected override SqlQuery CreateSqlQueryReturningStructureIds(IQuery query)
        {
			var sqlExpression = SqlExpressionBuilder.Process(query);

            var formatter = new SqlQueryFormatter
            {
				MainStructureTable = query.StructureSchema.GetStructureTableName(),
				WhereAndSortingJoins = GenerateWhereAndSortingJoins(query, sqlExpression),
                WhereCriteria = GenerateWhereCriteriaString(sqlExpression)
            };

            return new SqlQuery(formatter.Format(SqlStatements.GetSql("QueryReturningStructureIds")), sqlExpression.WhereCriteria.Parameters);
        }

		protected override string GenerateOrderByMembersString(IQuery query, ISqlExpression sqlExpression)
        {
			return query.HasPaging
                ? string.Empty
				: base.GenerateOrderByMembersString(query, sqlExpression);
        }

		protected override string GenerateOrderByString(IQuery query, ISqlExpression sqlExpression)
        {
			return query.HasPaging
                ? string.Empty
				: base.GenerateOrderByString(query, sqlExpression);
        }

		protected override string GeneratePagingString(IQuery query, ISqlExpression sqlExpression)
        {
			if (!query.HasPaging)
                return string.Empty;

            var s = string.Join(", ", sqlExpression.SortingMembers.Select(
				sorting => sorting.MemberPath != IndexStorageSchema.Fields.StructureId.Name 
					? string.Format("min(mem{0}.[{1}]) {2}", sorting.Index, sorting.IndexStorageColumnName, sorting.Direction)
					: string.Format("s.[{0}] {1}", IndexStorageSchema.Fields.StructureId.Name, sorting.Direction)));
            
            return string.Format("row_number() over (order by {0}) RowNum", s);
        }

		protected override string GenerateEndString(IQuery query, ISqlExpression sqlExpression)
        {
			return query.HasPaging
                ? "where rs.RowNum between @pagingFrom and @pagingTo"
                : string.Empty;
        }
    }
}