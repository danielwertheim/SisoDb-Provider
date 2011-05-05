using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SisoDb.Commands;
using SisoDb.Core;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Processors;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Sql2008
{
    public class SqlQueryGenerator : ISqlQueryGenerator
    {
        private readonly IParsedLambdaProcessor<ISqlWhere> _parsedWhereProcessor;
        private readonly IParsedLambdaProcessor<ISqlSorting> _parsedSortingProcessor;
        private readonly IParsedLambdaProcessor<IList<ISqlInclude>> _parsedIncludeProcessor;

        public SqlQueryGenerator(
            IParsedLambdaProcessor<ISqlWhere> parsedWhereProcessor,
            IParsedLambdaProcessor<ISqlSorting> parsedSortingProcessor,
            IParsedLambdaProcessor<IList<ISqlInclude>> parsedIncludeProcessor)
        {
            _parsedWhereProcessor = parsedWhereProcessor.AssertNotNull("parsedWhereProcessor");
            _parsedSortingProcessor = parsedSortingProcessor.AssertNotNull("parsedSortingProcessor");
            _parsedIncludeProcessor = parsedIncludeProcessor.AssertNotNull("parsedIncludeProcessor");
        }

        public ISqlCommandInfo Generate(IQueryCommand queryCommand, IStructureSchema schema)
        {
            queryCommand.AssertNotNull("queryCommand");
            schema.AssertNotNull("schema");

            var whereTuple = GenerateWhereStringAndParams(queryCommand);
            var buildInfo = new SqlCommandBuildInfo
            {
                StructureTableName = schema.GetStructureTableName(),
                IndexesTableName = schema.GetIndexesTableName(),
                TakeSql = GenerateTakeString(queryCommand),
                IncludesSql = GenerateIncludesString(queryCommand),
                OrderBySql = GenerateSortingString(queryCommand),
                WhereSql = whereTuple.Item1,
                WhereParams = whereTuple.Item2
            };

            return queryCommand.HasPaging 
                ? CreateSqlCommandInfoForPaging(buildInfo, queryCommand.Paging) 
                : CreateSqlCommandInfo(buildInfo);
        }

        private static ISqlCommandInfo CreateSqlCommandInfo(SqlCommandBuildInfo sqlCommandBuildInfo)
        {
            var sql = string.Format("select {0}s.Json{1} from [dbo].[{2}] as s inner join [dbo].[{3}] as si on si.SisoId = s.SisoId{4}{5};",
                sqlCommandBuildInfo.TakeSql,
                sqlCommandBuildInfo.IncludesSql,
                sqlCommandBuildInfo.StructureTableName, 
                sqlCommandBuildInfo.IndexesTableName, 
                sqlCommandBuildInfo.WhereSql, 
                sqlCommandBuildInfo.OrderBySql);

            return new SqlCommandInfo(sql, sqlCommandBuildInfo.WhereParams);
        }

        private static ISqlCommandInfo CreateSqlCommandInfoForPaging(SqlCommandBuildInfo sqlCommandBuildInfo, Paging paging)
        {
            var orderBySql = string.IsNullOrWhiteSpace(sqlCommandBuildInfo.OrderBySql)
                                 ? "order by s.SisoId"
                                 : sqlCommandBuildInfo.OrderBySql;
            const string sqlFormat = "with pagedRs as ({0}){1};";
            var innerSelect = string.Format("select {0}s.Json{1},row_number() over ({2}) RowNum from [dbo].[{3}] as s inner join [dbo].[{4}] as si on si.SisoId = s.SisoId{5}",
                sqlCommandBuildInfo.TakeSql,
                sqlCommandBuildInfo.IncludesSql,
                orderBySql,
                sqlCommandBuildInfo.StructureTableName,
                sqlCommandBuildInfo.IndexesTableName,
                sqlCommandBuildInfo.WhereSql);
            var outerSelect = string.Format("select Json{0} from pagedRs where RowNum between @pagingFrom and @pagingTo", sqlCommandBuildInfo.IncludesSql);
            var sql = string.Format(sqlFormat, innerSelect, outerSelect);

            var takeFromRowNum = (paging.PageIndex * paging.PageSize) + 1;
            var takeToRowNum = (takeFromRowNum + paging.PageSize) - 1;
            var queryParams = new List<IQueryParameter>(sqlCommandBuildInfo.WhereParams)
            {
                new QueryParameter("@pagingFrom", takeFromRowNum),
                new QueryParameter("@pagingTo", takeToRowNum)
            };

            return new SqlCommandInfo(sql, queryParams);
        }
        
        private static string GenerateTakeString(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasTakeNumOfStructures)
                return string.Empty;

            return string.Format("top({0}) ", queryCommand.TakeNumOfStructures);
        }

        private Tuple<string, IList<IQueryParameter>> GenerateWhereStringAndParams(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasWhere)
                return new Tuple<string, IList<IQueryParameter>>(string.Empty, new List<IQueryParameter>());

            var where = _parsedWhereProcessor.Process(queryCommand.Where);
            var sql = string.IsNullOrWhiteSpace(where.Sql)
                ? string.Empty
                : " where " + where.Sql;

            return new Tuple<string, IList<IQueryParameter>>(sql, where.Parameters);
        }

        private string GenerateSortingString(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasSortings)
                return string.Empty;

            var sorting = _parsedSortingProcessor.Process(queryCommand.Sortings);
            return string.IsNullOrWhiteSpace(sorting.Sql)
                ? string.Empty
                : " order by " + sorting.Sql;
        }

        private string GenerateIncludesString(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasIncludes)
                return string.Empty;

            var sb = new StringBuilder();
            var sqls = queryCommand.Includes
                .Select(parsedLambda => _parsedIncludeProcessor.Process(parsedLambda))
                .SelectMany(includes => includes.Select(include => include.Sql)).ToList();

            for (var c = 0; c < sqls.Count; c++)
            {
                sb.Append(sqls[c]);
                if (c < (sqls.Count - 1))
                    sb.Append(", ");
            }

            return sb.Length > 0 ? ", " + sb : string.Empty;
        }

        public ISqlCommandInfo GenerateWhere(IQueryCommand queryCommand)
        {
            queryCommand.AssertNotNull("queryCommand");

            if (!queryCommand.HasWhere)
                throw new ArgumentException(ExceptionMessages.SqlQueryGenerator_GenerateWhere);

            var where = _parsedWhereProcessor.Process(queryCommand.Where);
            var queryParameters = where.Parameters;

            return new SqlCommandInfo(where.Sql, queryParameters);
        }
    }
}