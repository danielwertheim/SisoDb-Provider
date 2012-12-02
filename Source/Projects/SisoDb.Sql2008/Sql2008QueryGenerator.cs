using System.Collections.Generic;
using System.Linq;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.NCore;
using SisoDb.Querying;
using SisoDb.Querying.Sql;

namespace SisoDb.Sql2008
{
    public class Sql2008QueryGenerator : DbQueryGenerator
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
                OrderByMembers = GenerateOrderByMembersString(query, sqlExpression),
                MainStructureTable = query.StructureSchema.GetStructureTableName(),
                WhereAndSortingJoins = GenerateWhereAndSortingJoins(query, sqlExpression),
                WhereCriteria = GenerateWhereCriteriaString(sqlExpression),
                OrderBy = GenerateOrderByString(query, sqlExpression),
                Paging = GeneratePagingString(query, sqlExpression).PrependWith(", "),
            };
        }

		protected override string GenerateOrderByMembersString(IQuery query, ISqlExpression sqlExpression)
        {
            return query.HasPaging || query.HasSkipNumOfStructures
                ? string.Empty
				: base.GenerateOrderByMembersString(query, sqlExpression);
        }

		protected override string GenerateOrderByString(IQuery query, ISqlExpression sqlExpression)
        {
            return query.HasPaging || query.HasSkipNumOfStructures
                ? string.Empty
				: base.GenerateOrderByString(query, sqlExpression);
        }

        protected override IDacParameter[] GenerateRsSizeLimitingParametersForPaging(IQuery query, ISqlExpression sqlExpression)
        {
            var skipRows = (query.Paging.PageIndex * query.Paging.PageSize);
            var takeRows = (skipRows + query.Paging.PageSize);

            return new IDacParameter[] 
            {
                new DacParameter("skipRows", skipRows),
                new DacParameter("takeRows", takeRows)
            };
        }

        protected override IDacParameter[] GenerateRsSizeLimitingParametersForSkipAndTake(IQuery query, ISqlExpression sqlExpression)
        {
            var skipRows = 0;
            var ps = new List<IDacParameter>(2);

            if (query.SkipNumOfStructures.HasValue)
            {
                skipRows = query.SkipNumOfStructures.Value;
                ps.Add(new DacParameter("skipRows", skipRows));
            }

            if (query.TakeNumOfStructures.HasValue)
            {
                var takeRows = skipRows + query.TakeNumOfStructures.Value;
                ps.Add(new DacParameter("takeRows", takeRows));
            }

            return ps.ToArray();
        }

        protected override string GeneratePagingString(IQuery query, ISqlExpression sqlExpression)
        {
            if (!query.HasPaging && !query.HasSkipNumOfStructures)
                return string.Empty;

            var s = string.Join(", ", sqlExpression.SortingMembers.Select(
                sorting => sorting.MemberPath != IndexStorageSchema.Fields.StructureId.Name
                    ? string.Format("min(mem{0}.[{1}]) {2}", sorting.Index, sorting.IndexStorageColumnName, sorting.Direction)
                    : string.Format("s.[{0}] {1}", StructureStorageSchema.Fields.Id.Name, sorting.Direction)));

            return string.Format("row_number() over (order by {0}) RowNum", s);
        }

        protected override string GenerateEndString(IQuery query, ISqlExpression sqlExpression)
        {
            if (!query.HasPaging && !query.HasSkipNumOfStructures)
                return string.Empty;

            if (query.HasPaging || (query.HasSkipNumOfStructures && query.HasTakeNumOfStructures))
                return "where rs.RowNum > @skipRows and rs.RowNum <= @takeRows";

            return "where rs.RowNum > @skipRows";
        }
    }
}