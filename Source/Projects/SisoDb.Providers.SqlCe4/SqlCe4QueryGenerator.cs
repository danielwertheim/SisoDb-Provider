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

        protected override SqlQuery CreateSqlCommandInfo(IQueryCommand queryCommand, IList<SqlInclude> includes, SqlWhere whereSql, IList<SqlSorting> sortings)
        {
            var sql = string.Format("select s.Json{0} from [{1}] as s inner join [{2}] as si on si.[StructureId] = s.[StructureId]{3} group by s.[StructureId], s.[Json] order by {4} offset 0 rows{5}",
                SqlSorting.ToColumnDefinitionString(sortings, "min({0})").PrependWith(", "),
                queryCommand.StructureSchema.GetStructureTableName(),
                queryCommand.StructureSchema.GetIndexesTableName(),
                whereSql.Sql.PrependWith(" where "),
                SqlSorting.ToAliasAndDirectionString(sortings),
                GenerateTakeString(queryCommand).PrependWith(" "));

            //TODO: SqlInclude.ToJoinString(includes).PrependWith(" "),

            var outerSql = string.Format("select rs.json{0} from ({1}) rs;",
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(", "),
                sql);

            return new SqlQuery(outerSql, whereSql.Parameters);
        }

        protected override SqlQuery CreateSqlCommandInfoForPaging(IQueryCommand queryCommand, IList<SqlInclude> includes, SqlWhere whereSql, IList<SqlSorting> sortings)
        {
            var offsetRows = (queryCommand.Paging.PageIndex * queryCommand.Paging.PageSize);
            var takeRows = queryCommand.Paging.PageSize;

            var sql = string.Format("select s.structureid, min(s.Json) Json{0} from [{1}] as s inner join [{2}] as si on si.[StructureId] = s.[StructureId] {3} group by s.[StructureId] order by {4} offset {5} rows fetch next {6} rows only",
                SqlSorting.ToColumnDefinitionString(sortings, "min({0})").PrependWith(", "),
                queryCommand.StructureSchema.GetStructureTableName(),
                queryCommand.StructureSchema.GetIndexesTableName(),
                whereSql.Sql.PrependWith("where "),
                SqlSorting.ToAliasAndDirectionString(sortings),
                offsetRows,
                takeRows);

            //TODO: SqlInclude.ToJoinString(includes).PrependWith(" "),

            var outerSql = string.Format("select rs.json{0} from ({1}) rs;",
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(", "),
                sql);

            return new SqlQuery(outerSql, whereSql.Parameters);
        }

        protected override string GenerateTakeString(IQueryCommand queryCommand)
        {
            return queryCommand.HasTakeNumOfStructures ? string.Format("fetch next {0} rows only", queryCommand.TakeNumOfStructures) : string.Empty;
        }
    }
}