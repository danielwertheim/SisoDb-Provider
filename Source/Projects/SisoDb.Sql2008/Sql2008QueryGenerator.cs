using System.Linq;
using NCore;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying.Sql;
using SisoDb.SqlServer;

namespace SisoDb.Sql2008
{
    public class Sql2008QueryGenerator : SqlServerQueryGenerator
    {
        public Sql2008QueryGenerator(ISqlStatements sqlStatements, ISqlExpressionBuilder sqlExpressionBuilder) 
            : base(sqlStatements, sqlExpressionBuilder) {}

        protected override SqlQueryFormatter CreateSqlQueryFormatter(IQuery query, ISqlExpression sqlExpression)
        {
            return new SqlQueryFormatter
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
                Paging = GeneratePagingString(query, sqlExpression).PrependWith(", "),
            };
        }

        protected override IDacParameter[] GeneratePagingParameters(IQuery query, ISqlExpression sqlExpression)
        {
            if (!query.HasPaging)
                return new IDacParameter[0];

            var takeFromRowNum = (query.Paging.PageIndex * query.Paging.PageSize) + 1;
            var takeToRowNum = (takeFromRowNum + query.Paging.PageSize) - 1;

            return new[]
            {
                new DacParameter("pagingFrom", takeFromRowNum),
                new DacParameter("pagingTo", takeToRowNum)
            };
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