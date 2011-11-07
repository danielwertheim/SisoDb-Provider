using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using NCore;
using SisoDb.Core;
using SisoDb.Core.Collections;
using SisoDb.Dac;
using SisoDb.Querying.Lambdas;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Structures;

namespace SisoDb.Querying
{
    public abstract class DbQueryGenerator : IDbQueryGenerator
    {
        protected readonly ILambdaToSqlWhereConverter WhereConverter;
        protected readonly ILambdaToSqlSortingConverter SortingConverter;
        protected readonly ILambdaToSqlIncludeConverter IncludeConverter;

        protected DbQueryGenerator(
            ILambdaToSqlWhereConverter whereConverter,
            ILambdaToSqlSortingConverter sortingConverter,
            ILambdaToSqlIncludeConverter includeConverter)
        {
            Ensure.That(whereConverter, "whereConverter").IsNotNull();
            Ensure.That(sortingConverter, "sortingConverter").IsNotNull();
            Ensure.That(includeConverter, "includeConverter").IsNotNull();

            WhereConverter = whereConverter;
            SortingConverter = sortingConverter;
            IncludeConverter = includeConverter;
        }

        public SqlQuery GenerateQuery(IQueryCommand queryCommand)
        {
            Ensure.That(queryCommand, "queryCommand").IsNotNull();

            return queryCommand.HasPaging
                ? CreateSqlCommandInfoForPaging(queryCommand)
                : CreateSqlCommandInfo(queryCommand);
        }

        public SqlQuery GenerateWhereQuery(IQueryCommand queryCommand)
        {
            Ensure.That(queryCommand, "queryCommand").IsNotNull();

            if (!queryCommand.HasWhere || (queryCommand.HasIncludes || queryCommand.HasSortings || queryCommand.HasPaging || queryCommand.HasTakeNumOfStructures))
                throw new ArgumentException(ExceptionMessages.DbQueryGenerator_GenerateWhere);

            return CreateSqlCommandInfo(queryCommand);
        }

        protected virtual SqlQuery CreateSqlCommandInfo(IQueryCommand queryCommand)
        {
            var includes = GenerateIncludes(queryCommand);
            var where = WhereConverter.Convert(queryCommand.StructureSchema, queryCommand.Where);
            var sortings = SortingConverter.Convert(queryCommand.StructureSchema, queryCommand.Sortings);

            var sql = string.Format(
                "select {0}s.Json{1} from [{2}] s{3}{4} inner join [{5}] si on si.[StructureId] = s.[StructureId]{6}{7} group by s.[StructureId], s.Json{1}{8};",
                GenerateTakeString(queryCommand).AppendWith(" "),
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(", "),
                queryCommand.StructureSchema.GetStructureTableName(),
                GenerateWhereJoinsString(queryCommand, where).PrependWith(" "),
                SqlInclude.ToJoinString(includes).PrependWith(" "),
                queryCommand.StructureSchema.GetIndexesTableName(),
                GenerateMemberPathsStringForJoin(where, sortings).PrependWith(" and si.[MemberPath] in(").AppendWith(")"),
                where.CriteriaString.PrependWith(" where "),
                SqlSorting.ToSortingString(sortings, "min({0})").PrependWith(" order by "));

            return new SqlQuery(sql, where.Parameters.ToArray());
        }

        protected virtual SqlQuery CreateSqlCommandInfoForPaging(IQueryCommand queryCommand)
        {
            var includes = GenerateIncludes(queryCommand);
            var where = WhereConverter.Convert(queryCommand.StructureSchema, queryCommand.Where);
            var sortings = SortingConverter.Convert(queryCommand.StructureSchema, queryCommand.Sortings);

            var innerSelect = string.Format(
                "select {0}s.Json{1}, row_number() over (order by {7}) as RowNum from [{2}] s{3}{4} inner join [{5}] si on si.[StructureId] = s.[StructureId]{6}{8} group by s.[StructureId], s.Json{1}",
                GenerateTakeString(queryCommand).AppendWith(" "),
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(", "),
                queryCommand.StructureSchema.GetStructureTableName(),
                GenerateWhereJoinsString(queryCommand, where).PrependWith(" "),
                SqlInclude.ToJoinString(includes).PrependWith(" "),
                queryCommand.StructureSchema.GetIndexesTableName(),
                GenerateMemberPathsStringForJoin(where, sortings).PrependWith(" and si.[MemberPath] in(").AppendWith(")"),
                queryCommand.HasSortings ? SqlSorting.ToSortingString(sortings, "min({0})") : "s.[StructureId]",
                where.CriteriaString.PrependWith(" where "));

            var sql = string.Format("with pagedRs as ({0}) pagedRs select {1}pagedRs.Json{2} from pagedRs where pagedRs.RowNum between @pagingFrom and @pagingTo;",
                innerSelect,
                GenerateTakeString(queryCommand).AppendWith(" "),
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(", "));

            var takeFromRowNum = (queryCommand.Paging.PageIndex * queryCommand.Paging.PageSize) + 1;
            var takeToRowNum = (takeFromRowNum + queryCommand.Paging.PageSize) - 1;
            var queryParams = new List<IDacParameter>(where.Parameters)
            {
                new DacParameter("@pagingFrom", takeFromRowNum),
                new DacParameter("@pagingTo", takeToRowNum)
            };

            return new SqlQuery(sql, queryParams);
        }

        protected virtual string GenerateTakeString(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasTakeNumOfStructures)
                return string.Empty;

            return string.Format("top({0})", queryCommand.TakeNumOfStructures);
        }

        protected virtual string GenerateMemberPathsStringForJoin(SqlWhere where, IEnumerable<SqlSorting> sortings)
        {
            return where.MemberPaths.MergeDistinctWith(sortings.Select(s => s.MemberPath)).ToJoinedString(", ", "'{0}'");
        }

        protected virtual string GenerateWhereJoinsString(IQueryCommand queryCommand, SqlWhere where)
        {
            var indexesTableName = queryCommand.StructureSchema.GetIndexesTableName();

            var joins = new List<string>(where.MemberPaths.Length);

            foreach (var memberPath in where.MemberPaths)
            {
                joins.Add(string.Format("left join [{0}] as mem{1} on mem{1}.[StructureId]=s.[StructureId] and mem{1}.[MemberPath]='{2}'", 
                    indexesTableName,
                    joins.Count,
                    memberPath));
            }

            return string.Join(" ", joins);
        }

        protected virtual IList<SqlInclude> GenerateIncludes(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasIncludes)
                return new List<SqlInclude>();

            IParsedLambda mergedIncludes = null;
            foreach (var include in queryCommand.Includes)
            {
                if (mergedIncludes == null)
                    mergedIncludes = include;
                else
                    mergedIncludes = mergedIncludes.MergeAsNew(include);
            }

            return IncludeConverter.Convert(queryCommand.StructureSchema, mergedIncludes);
        }
    }
}