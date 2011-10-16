using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnsureThat;
using SisoDb.Dac;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.Resources;
using SisoDb.Structures;

namespace SisoDb.Querying.Sql
{
    public class SqlQueryGenerator : IDbQueryGenerator
    {
        private readonly ILambdaToSqlWhereConverter _whereConverter;
        private readonly ILambdaToSqlSortingConverter _sortingConverter;
        private readonly ILambdaToSqlIncludeConverter _includeConverter;

        public SqlQueryGenerator(
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

        private SqlQuery CreateSqlCommandInfo(IQueryCommand queryCommand)
        {
            var whereSql = GenerateWhereStringAndParams(queryCommand);
            var sortingSql = GenerateSortingString(queryCommand);

            var sql = string.Format("select {0}s.Json{1} from [dbo].[{2}] as s inner join [dbo].[{3}] as si on si.[StructureId] = s.[StructureId]{4}{5}{6};",
                GenerateTakeString(queryCommand),
                GenerateIncludesString(queryCommand),
                queryCommand.StructureSchema.GetStructureTableName(), 
                queryCommand.StructureSchema.GetIndexesTableName(),
                sortingSql.SortingJoins,
                whereSql.Sql,
                sortingSql.Sorting);

            return new SqlQuery(sql, whereSql.Parameters);
        }

        private SqlQuery CreateSqlCommandInfoForPaging(IQueryCommand queryCommand)
        {
            var includesSql = GenerateIncludesString(queryCommand);
            var whereSql = GenerateWhereStringAndParams(queryCommand);
            var sortingSql = GenerateSortingString(queryCommand, defaultSort: "s.StructureId");
            
            const string sqlFormat = "with pagedRs as ({0}){1};";
            
            var innerSelect = string.Format("select {0}s.Json{1},row_number() over ({2}) RowNum from [dbo].[{3}] as s inner join [dbo].[{4}] as si on si.[StructureId] = s.[StructureId]{5}{6}",
                GenerateTakeString(queryCommand),
                includesSql,
                sortingSql.Sorting,
                queryCommand.StructureSchema.GetStructureTableName(),
                queryCommand.StructureSchema.GetIndexesTableName(),
                sortingSql.SortingJoins,
                whereSql.Sql);
            
            var outerSelect = string.Format("select Json{0} from pagedRs where RowNum between @pagingFrom and @pagingTo", includesSql);
            
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

        private SqlWhere GenerateWhereStringAndParams(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasWhere)
                return SqlWhere.Empty();

            var where = _whereConverter.Convert(queryCommand.StructureSchema, queryCommand.Where);
            var sql = string.IsNullOrWhiteSpace(where.Sql)
                ? string.Empty
                : " where " + where.Sql;

            return new SqlWhere(sql, where.Parameters);
        }

        private SqlSorting GenerateSortingString(IQueryCommand queryCommand, string defaultSort = null)
        {
            if (!queryCommand.HasSortings)
                return defaultSort == null ? SqlSorting.Empty() : new SqlSorting(" order by " + defaultSort);

            var sorting = _sortingConverter.Convert(queryCommand.StructureSchema, queryCommand.Sortings);

            return sorting.IsEmpty
                ? SqlSorting.Empty()
                : new SqlSorting(" order by " + sorting.Sorting,  " " + sorting.SortingJoins);
        }

        private string GenerateIncludesString(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasIncludes)
                return string.Empty;

            var sb = new StringBuilder();
            var sqls = queryCommand.Includes
                .Select(parsedLambda => _includeConverter.Convert(queryCommand.StructureSchema, parsedLambda))
                .SelectMany(includes => includes.Select(include => include.Sql)).ToList();

            for (var c = 0; c < sqls.Count; c++)
            {
                sb.Append(sqls[c]);
                if (c < (sqls.Count - 1))
                    sb.Append(", ");
            }

            return sb.Length > 0 ? ", " + sb : string.Empty;
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
    }
}