using System.Linq;
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

        protected override SqlQuery CreateSqlCommandInfo(IQueryCommand queryCommand)
        {
            var includes = GenerateIncludes(queryCommand);
            var wheres = WhereConverter.Convert(queryCommand.StructureSchema, queryCommand.Where);
            var sortings = SortingConverter.Convert(queryCommand.StructureSchema, queryCommand.Sortings);

            var sql = string.Format(
                "select {0}s.Json{1}{2} from [{3}] s{4}{5} inner join [{6}] si on si.[StructureId] = s.[StructureId]{7}group by s.[StructureId], s.Json{2}{8};",
                GenerateTakeString(queryCommand).AppendWith(" "),
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(", "),
                SqlSorting.ToColumnDefinitionString(sortings, "min({0})").PrependWith(", "),
                queryCommand.StructureSchema.GetStructureTableName(),
                SqlWhere.ToJoinString(wheres).PrependWith(" "),
                SqlInclude.ToJoinString(includes).PrependWith(" "),
                queryCommand.StructureSchema.GetIndexesTableName(),
                GenerateMemberPathsStringForJoin(wheres, sortings).PrependWith(" and si.[MemberPath] in(").AppendWith(")"),
                SqlSorting.ToAliasAndDirectionString(sortings).PrependWith(" order by "));

            return new SqlQuery(sql, wheres.SelectMany(w => w.Parameters).ToArray());
        }

        protected override SqlQuery CreateSqlCommandInfoForPaging(IQueryCommand queryCommand)
        {
            //TODO: Turn paging vars to parameters

            var includes = GenerateIncludes(queryCommand);
            var wheres = WhereConverter.Convert(queryCommand.StructureSchema, queryCommand.Where);
            var sortings = SortingConverter.Convert(queryCommand.StructureSchema, queryCommand.Sortings);

            var offsetRows = (queryCommand.Paging.PageIndex * queryCommand.Paging.PageSize);
            
            var sql = string.Format(
                "select s.Json{0}{1} from [{2}] s{3}{4} inner join [{5}] si on si.[StructureId] = s.[StructureId]{6}group by s.[StructureId], s.Json{1} order by {7} offset {8} rows{9};",
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(", "),
                SqlSorting.ToColumnDefinitionString(sortings, "min({0})").PrependWith(", "),
                queryCommand.StructureSchema.GetStructureTableName(),
                SqlWhere.ToJoinString(wheres).PrependWith(" "),
                SqlInclude.ToJoinString(includes).PrependWith(" "),
                queryCommand.StructureSchema.GetIndexesTableName(),
                GenerateMemberPathsStringForJoin(wheres, sortings).PrependWith(" and si.[MemberPath] in(").AppendWith(")"),
                queryCommand.HasSortings ? SqlSorting.ToAliasAndDirectionString(sortings) : "s.[StructureId]",
                offsetRows,
                GenerateFetchString(queryCommand).AppendWith(" "));

            return new SqlQuery(sql, wheres.SelectMany(w => w.Parameters).ToArray());
        }

        protected virtual string GenerateFetchString(IQueryCommand queryCommand)
        {
            var takeRows = queryCommand.TakeNumOfStructures;

            if (queryCommand.HasPaging)
                takeRows = queryCommand.Paging.PageSize;

            return takeRows > 0 ? string.Format("fetch next {0} rows only", queryCommand.TakeNumOfStructures) : string.Empty;
        }
    }
}