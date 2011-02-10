using System;
using System.Collections.Generic;
using SisoDb.Lambdas;
using SisoDb.Lambdas.Processors;
using SisoDb.Querying;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    internal class SqlQueryGenerator : ISqlQueryGenerator
    {
        private readonly ISelectorParser _selectorParser;
        private readonly ISortingParser _sortingParser;
        private readonly IParsedLambdaProcessor<ISqlSelector> _parsedSelectorProcessor;
        private readonly IParsedLambdaProcessor<ISqlSorting> _parsedSortingProcessor;

        internal SqlQueryGenerator(ISelectorParser selectorParser, ISortingParser sortingParser, IParsedLambdaProcessor<ISqlSelector> parsedSelectorProcessor, IParsedLambdaProcessor<ISqlSorting> parsedSortingProcessor)
        {
            _selectorParser = selectorParser.AssertNotNull("selectorParser");
            _sortingParser = sortingParser;
            _parsedSelectorProcessor = parsedSelectorProcessor.AssertNotNull("parsedSelectorProcessor");
            _parsedSortingProcessor = parsedSortingProcessor.AssertNotNull("parsedSortingProcessor");
        }

        public ISqlCommandInfo Generate<T>(IQueryCommand<T> queryCommand, IStructureSchema schema) where T : class
        {
            queryCommand.AssertNotNull("queryCommand");
            schema.AssertNotNull("schema");

            var where = string.Empty;
            var orderBy = string.Empty;
            IList<IQueryParameter> queryParameters = new List<IQueryParameter>();

            if (queryCommand.HasSelector)
            {
                var parsedSelectorLambda = _selectorParser.Parse(queryCommand.Selector);
                var selector = _parsedSelectorProcessor.Process(parsedSelectorLambda);
                where = !string.IsNullOrWhiteSpace(selector.Sql) ? " where " + selector.Sql : string.Empty;
                queryParameters = selector.Parameters;
            }

            if(queryCommand.HasSortings)
            {
                var parsedSortingLambda = _sortingParser.Parse(queryCommand.Sortings);
                var sorting = _parsedSortingProcessor.Process(parsedSortingLambda);
                orderBy = !string.IsNullOrWhiteSpace(sorting.Sql) ? " order by " + sorting.Sql : string.Empty;
            }
            
            var structureTableName = schema.GetStructureTableName();
            var indexesTableName = schema.GetIndexesTableName();
            
            var sql =
                string.Format(
                    "select s.Json from [dbo].[{0}] as s inner join [dbo].[{1}] as si on si.StructureId = s.Id{2}{3};",
                    structureTableName, indexesTableName, where, orderBy);

            return new SqlCommandInfo(sql, queryParameters);
        }

        public ISqlCommandInfo GenerateWhere<T>(IQueryCommand<T> queryCommand, IStructureSchema schema) where T : class
        {
            queryCommand.AssertNotNull("queryCommand");
            schema.AssertNotNull("schema");

            if (!queryCommand.HasSelector)
                throw new ArgumentException("A where clause can not be generated if the query command doesn't contain a selector."); //TODO: Resource

            var parsedSelectorLambda = _selectorParser.Parse(queryCommand.Selector);
            var selector = _parsedSelectorProcessor.Process(parsedSelectorLambda);
            var queryParameters = selector.Parameters;

            return new SqlCommandInfo(selector.Sql, queryParameters);
        }
    }
}