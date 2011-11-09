using System.Collections.Generic;
using NCore;
using SisoDb.Dac;
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

        protected override SqlQuery CreateSqlQuery(IQueryCommand queryCommand)
        {
            var includes = GenerateIncludes(queryCommand);
            var where = WhereConverter.Convert(queryCommand.StructureSchema, queryCommand.Where);
            var sortings = SortingConverter.Convert(queryCommand.StructureSchema, queryCommand.Sortings);

            var sql = string.Format(
                "select {0}s.Json{1}{2} from [{3}] s inner join [{6}] si on si.[StructureId] = s.[StructureId]{7}{4}{5}{8}{9}{10};",
                GenerateTakeString(queryCommand).AppendWith(" "),
                SqlInclude.ToJsonOutputDefinitionString(includes).PrependWith(", "),
                SqlSorting.ToColumnDefinitionString(sortings, "min({0})").PrependWith(", "),
                queryCommand.StructureSchema.GetStructureTableName(),
                GenerateWhereJoinsString(queryCommand, where).PrependWith(" "),
                SqlInclude.ToJoinString(includes).PrependWith(" "),
                queryCommand.StructureSchema.GetIndexesTableName(),
                GenerateMemberPathsStringForJoin(where, sortings).PrependWith(" and si.[MemberPath] in(").AppendWith(")"),
                where.CriteriaString.PrependWith(" where "),
                GenerateGroupingMembersString(queryCommand).PrependWith(" group by "),
                SqlSorting.ToAliasAndDirectionString(sortings).PrependWith(" order by "));

            return new SqlQuery(sql, where.Parameters);
        }

        protected override SqlQuery CreateSqlQueryForPaging(IQueryCommand queryCommand)
        {
            var includes = GenerateIncludes(queryCommand);
            var where = WhereConverter.Convert(queryCommand.StructureSchema, queryCommand.Where);
            var sortings = SortingConverter.Convert(queryCommand.StructureSchema, queryCommand.Sortings);

            var offsetRows = (queryCommand.Paging.PageIndex * queryCommand.Paging.PageSize);
            var takeRows = queryCommand.TakeNumOfStructures;
            if (queryCommand.HasPaging)
                takeRows = queryCommand.Paging.PageSize;

            var sql = string.Format(
                "select s.Json{0}{1} from [{2}] s inner join [{5}] si on si.[StructureId] = s.[StructureId]{6}{3}{4}{7}{8} order by {9} offset @offsetRows rows{10};",
                SqlInclude.ToJsonOutputDefinitionString(includes).PrependWith(", "),
                SqlSorting.ToColumnDefinitionString(sortings, "min({0})").PrependWith(", "),
                queryCommand.StructureSchema.GetStructureTableName(),
                GenerateWhereJoinsString(queryCommand, where).PrependWith(" "),
                SqlInclude.ToJoinString(includes).PrependWith(" "),
                queryCommand.StructureSchema.GetIndexesTableName(),
                GenerateMemberPathsStringForJoin(where, sortings).PrependWith(" and si.[MemberPath] in(").AppendWith(")"),
                where.CriteriaString.PrependWith(" where "),
                GenerateGroupingMembersString(queryCommand).PrependWith(" group by "),
                queryCommand.HasSortings ? SqlSorting.ToAliasAndDirectionString(sortings) : "s.[StructureId]",
                GenerateFetchString(queryCommand).PrependWith(" "));

            var queryParams = new List<IDacParameter>(where.Parameters)
            {
                new DacParameter("@offsetRows", offsetRows),
                new DacParameter("@takeRows", takeRows)
            };

            return new SqlQuery(sql, queryParams);
        }

        protected virtual string GenerateFetchString(IQueryCommand queryCommand)
        {
            return queryCommand.HasTakeNumOfStructures || queryCommand.HasPaging
                ? "fetch next @takeRows rows only" 
                : string.Empty;
        }
    }
}