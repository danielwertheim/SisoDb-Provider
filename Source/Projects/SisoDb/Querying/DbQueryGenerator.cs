using System;
using System.Collections.Generic;
using EnsureThat;
using NCore;
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
        private readonly ILambdaToSqlWhereConverter _whereConverter;
        private readonly ILambdaToSqlSortingConverter _sortingConverter;
        private readonly ILambdaToSqlIncludeConverter _includeConverter;

        protected DbQueryGenerator(
            ILambdaToSqlWhereConverter whereConverter,
            ILambdaToSqlSortingConverter sortingConverter,
            ILambdaToSqlIncludeConverter includeConverter)
        {
            Ensure.That(whereConverter, "whereConverter").IsNotNull();
            Ensure.That(sortingConverter, "sortingConverter").IsNotNull();
            Ensure.That(includeConverter, "includeConverter").IsNotNull();

            _whereConverter = whereConverter;
            _sortingConverter = sortingConverter;
            _includeConverter = includeConverter;
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

            if (!queryCommand.HasWhere)
                throw new ArgumentException(ExceptionMessages.SqlQueryGenerator_GenerateWhere);

            var where = _whereConverter.Convert(queryCommand.StructureSchema, queryCommand.Where);
            var queryParameters = where.Parameters;

            return new SqlQuery(where.Sql, queryParameters);
        }

        private SqlQuery CreateSqlCommandInfo(IQueryCommand queryCommand)
        {
            var includes = GenerateIncludes(queryCommand);
            var whereSql = GenerateWhere(queryCommand);
            var sortingSql = GenerateSorting(queryCommand);

            var sql = string.Format("select {0}s.Json{1} from [{2}] as s inner join [{3}] as si on si.[StructureId] = s.[StructureId]{4}{5} group by s.[StructureId], s.[Json]{6};",
                GenerateTakeString(queryCommand),
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(","),
                queryCommand.StructureSchema.GetStructureTableName(), 
                queryCommand.StructureSchema.GetIndexesTableName(),
                SqlInclude.ToJoinString(includes).PrependWith(" "),
                whereSql.Sql,
                sortingSql.Sorting);

            return new SqlQuery(sql, whereSql.Parameters);
        }

        private SqlQuery CreateSqlCommandInfoForPaging(IQueryCommand queryCommand)
        {
            var includes = GenerateIncludes(queryCommand);
            var whereSql = GenerateWhere(queryCommand);
            var sortingSql = GenerateSorting(queryCommand);
            
            const string sqlFormat = "with pagedRs as ({0}){1};";

            var innerSelect = string.Format("select {0}s.Json{1},row_number() over ({2}) RowNum from [{3}] as s inner join [{4}] as si on si.[StructureId] = s.[StructureId]{5}{6} group by s.[StructureId], s.[Json]",
                GenerateTakeString(queryCommand),
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(","),
                sortingSql.Sorting,
                queryCommand.StructureSchema.GetStructureTableName(),
                queryCommand.StructureSchema.GetIndexesTableName(),
                SqlInclude.ToJoinString(includes).PrependWith(" "),
                whereSql.Sql);

            var outerSelect = string.Format("select Json{0} from pagedRs where RowNum between @pagingFrom and @pagingTo", 
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(","));
            
            var sql = string.Format(sqlFormat, innerSelect, outerSelect);

            var takeFromRowNum = (queryCommand.Paging.PageIndex * queryCommand.Paging.PageSize) + 1;
            var takeToRowNum = (takeFromRowNum + queryCommand.Paging.PageSize) - 1;
            var queryParams = new List<IDacParameter>(whereSql.Parameters)
            {
                new DacParameter("@pagingFrom", takeFromRowNum),
                new DacParameter("@pagingTo", takeToRowNum)
            };

            return new SqlQuery(sql, queryParams);
        }

        private static string GenerateTakeString(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasTakeNumOfStructures)
                return string.Empty;

            return string.Format("top({0}) ", queryCommand.TakeNumOfStructures);
        }

        private IList<SqlInclude> GenerateIncludes(IQueryCommand queryCommand)
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

            return _includeConverter.Convert(queryCommand.StructureSchema, mergedIncludes);
        }

        private SqlWhere GenerateWhere(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasWhere)
                return SqlWhere.Empty();

            var where = _whereConverter.Convert(queryCommand.StructureSchema, queryCommand.Where);
            var sql = string.IsNullOrWhiteSpace(where.Sql)
                ? string.Empty
                : " where " + where.Sql;

            return new SqlWhere(sql, where.Parameters);
        }

        private SqlSorting GenerateSorting(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasSortings)
                return new SqlSorting(" order by s.[StructureId]");

            var sorting = _sortingConverter.Convert(queryCommand.StructureSchema, queryCommand.Sortings);

            return sorting.IsEmpty
                ? new SqlSorting(" order by s.[StructureId]")
                : new SqlSorting(" order by " + sorting.Sorting);
        }
    }
}