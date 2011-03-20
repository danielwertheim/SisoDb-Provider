using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SisoDb.Lambdas.Processors;
using SisoDb.Querying;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
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

            var takeSql = GenerateTakeString(queryCommand);
            var whereTuple = GenerateWhereStringAndParams(queryCommand);
            var whereSql = whereTuple.Item1;
            var whereParams = whereTuple.Item2;
            var orderBySql = GenerateSortingString(queryCommand);
            var includesSql = GenerateIncludesString(queryCommand);
            
            var structureTableName = schema.GetStructureTableName();
            var indexesTableName = schema.GetIndexesTableName();
            
            var sql =
                string.Format(
                    "select {2}s.Json{3} from [dbo].[{0}] as s inner join [dbo].[{1}] as si on si.StructureId = s.Id{4}{5};",
                    structureTableName, indexesTableName, takeSql, includesSql, whereSql, orderBySql);

            return new SqlCommandInfo(sql, whereParams);
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
            if(!queryCommand.HasIncludes)
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

        public ISqlCommandInfo GenerateWhere(IQueryCommand queryCommand, IStructureSchema schema)
        {
            queryCommand.AssertNotNull("queryCommand");
            schema.AssertNotNull("schema");

            if (!queryCommand.HasWhere)
                throw new ArgumentException(ExceptionMessages.SqlQueryGenerator_GenerateWhere);

            var where = _parsedWhereProcessor.Process(queryCommand.Where);
            var queryParameters = where.Parameters;

            return new SqlCommandInfo(where.Sql, queryParameters);
        }
    }
}