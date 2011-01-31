using System;
using System.Linq.Expressions;
using SisoDb.Lambdas;
using SisoDb.Lambdas.Processors;
using SisoDb.Querying;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    internal class SqlQueryGenerator : ISqlQueryGenerator
    {
        private readonly ILambdaParser _lambdaParser;
        private readonly IParsedLambdaProcessor<ISqlQuery> _parsedLambdaProcessor;

        internal SqlQueryGenerator(ILambdaParser lambdaParser, IParsedLambdaProcessor<ISqlQuery> parsedLambdaProcessor)
        {
            _lambdaParser = lambdaParser.AssertNotNull("lambdaParser");
            _parsedLambdaProcessor = parsedLambdaProcessor.AssertNotNull("parsedLambdaProcessor");
        }

        public ISqlQuery Generate<T>(Expression<Func<T, bool>> e, IStructureSchema schema) where T : class
        {
            var parsedLambda = _lambdaParser.Parse(e);
            var whereQuery = _parsedLambdaProcessor.Process(parsedLambda);

            var structureTableName = schema.GetStructureTableName();
            var indexesTableName = schema.GetIndexesTableName();

            var sql =
                string.Format(
                    "select s.Json from [dbo].[{0}] as s inner join [dbo].[{1}] as si on si.StructureId = s.Id where {2}",
                    structureTableName, indexesTableName, whereQuery.Sql);

            return new SqlQuery(sql, whereQuery.Parameters);
        }
    }
}