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
        private readonly IParsedLambdaProcessor<ISqlSelector> _parsedSelectorProcessor;
        private readonly IParsedLambdaProcessor<ISqlSorting> _parsedSortingProcessor;
        private readonly IParsedLambdaProcessor<IList<ISqlInclude>> _parsedIncludeProcessor;

        public SqlQueryGenerator(
            IParsedLambdaProcessor<ISqlSelector> parsedSelectorProcessor,
            IParsedLambdaProcessor<ISqlSorting> parsedSortingProcessor,
            IParsedLambdaProcessor<IList<ISqlInclude>> parsedIncludeProcessor)
        {
            _parsedSelectorProcessor = parsedSelectorProcessor.AssertNotNull("parsedSelectorProcessor");
            _parsedSortingProcessor = parsedSortingProcessor.AssertNotNull("parsedSortingProcessor");
            _parsedIncludeProcessor = parsedIncludeProcessor.AssertNotNull("parsedIncludeProcessor");
        }

        public ISqlCommandInfo Generate(IQueryCommand queryCommand, IStructureSchema schema)
        {
            queryCommand.AssertNotNull("queryCommand");
            schema.AssertNotNull("schema");

            var selector = GenerateSelectorStringAndParams(queryCommand);
            var where = selector.Item1;
            var queryParameters = selector.Item2;
            var orderBy = GenerateSortingString(queryCommand);
            var includes = GenerateIncludesString(queryCommand);
            
            var structureTableName = schema.GetStructureTableName();
            var indexesTableName = schema.GetIndexesTableName();
            
            var sql =
                string.Format(
                    "select s.Json{4} from [dbo].[{0}] as s inner join [dbo].[{1}] as si on si.StructureId = s.Id{2}{3};",
                    structureTableName, indexesTableName, where, orderBy, includes);

            return new SqlCommandInfo(sql, queryParameters);
        }

        private Tuple<string, IList<IQueryParameter>> GenerateSelectorStringAndParams(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasSelector)
                return new Tuple<string, IList<IQueryParameter>>(string.Empty, new List<IQueryParameter>());

            var selector = _parsedSelectorProcessor.Process(queryCommand.Selector);
            var sql = string.IsNullOrWhiteSpace(selector.Sql) 
                ? string.Empty 
                : " where " + selector.Sql;

            return new Tuple<string, IList<IQueryParameter>>(sql, selector.Parameters);
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

            if (!queryCommand.HasSelector)
                throw new ArgumentException(ExceptionMessages.SqlQueryGenerator_GenerateWhere);

            var selector = _parsedSelectorProcessor.Process(queryCommand.Selector);
            var queryParameters = selector.Parameters;

            return new SqlCommandInfo(selector.Sql, queryParameters);
        }
    }
}