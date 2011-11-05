using System.Collections.Generic;
using NCore;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.Querying.Sql;
using SisoDb.Structures;

namespace SisoDb.SqlCe4
{
    public class SqlCe4QueryGenerator : DbQueryGenerator
    {
        public SqlCe4QueryGenerator(
            ILambdaToSqlWhereConverter whereConverter,
            ILambdaToSqlSortingConverter sortingConverter,
            ILambdaToSqlIncludeConverter includeConverter) : base(whereConverter, sortingConverter, includeConverter)
        {
        }

        protected override SqlQuery CreateSqlCommandInfoForPaging(IQueryCommand queryCommand, IList<SqlInclude> includes, SqlWhere whereSql, SqlSorting sortingSql)
        {
            var offsetRows = (queryCommand.Paging.PageIndex * queryCommand.Paging.PageSize);
            var takeRows = queryCommand.Paging.PageSize;

            var sql = string.Format("select {0}s.Json{1} from [{2}] as s inner join [{3}] as si on si.[StructureId] = s.[StructureId]{4}{5} group by s.[StructureId], s.[Json]{6}{7};",
                GenerateTakeString(queryCommand),
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(","),
                queryCommand.StructureSchema.GetStructureTableName(),
                queryCommand.StructureSchema.GetIndexesTableName(),
                SqlInclude.ToJoinString(includes).PrependWith(" "),
                whereSql.Sql,
                sortingSql.Sorting,
                " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY".Inject(offsetRows, takeRows));

            return new SqlQuery(sql, whereSql.Parameters);

            //const string sqlFormat = "with pagedRs as ({0}){1};";

            //var innerSelect = string.Format("select {0}s.Json{1},row_number() over ({2}) RowNum from [{3}] as s inner join [{4}] as si on si.[StructureId] = s.[StructureId]{5}{6} group by s.[StructureId], s.[Json]",
            //    GenerateTakeString(queryCommand),
            //    SqlInclude.ToColumnDefinitionString(includes).PrependWith(","),
            //    sortingSql.Sorting,
            //    queryCommand.StructureSchema.GetStructureTableName(),
            //    queryCommand.StructureSchema.GetIndexesTableName(),
            //    SqlInclude.ToJoinString(includes).PrependWith(" "),
            //    whereSql.Sql);

            //var outerSelect = string.Format("select Json{0} from pagedRs where RowNum between @pagingFrom and @pagingTo",
            //    SqlInclude.ToColumnDefinitionString(includes).PrependWith(","));

            //var sql = string.Format(sqlFormat, innerSelect, outerSelect);

            //var takeFromRowNum = (queryCommand.Paging.PageIndex * queryCommand.Paging.PageSize) + 1;
            //var takeToRowNum = (takeFromRowNum + queryCommand.Paging.PageSize) - 1;
            //var queryParams = new List<IDacParameter>(whereSql.Parameters)
            //{
            //    new DacParameter("@pagingFrom", takeFromRowNum),
            //    new DacParameter("@pagingTo", takeToRowNum)
            //};

            //return new SqlQuery(sql, queryParams);
        }
    }
}