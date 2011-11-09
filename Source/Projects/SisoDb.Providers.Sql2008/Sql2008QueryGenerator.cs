using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Converters.Sql;

namespace SisoDb.Sql2008
{
    public class Sql2008QueryGenerator : DbQueryGenerator
    {
        public Sql2008QueryGenerator(
            ILambdaToSqlWhereConverter whereConverter,
            ILambdaToSqlSortingConverter sortingConverter,
            ILambdaToSqlIncludeConverter includeConverter) : base(whereConverter, sortingConverter, includeConverter)
        {
        }
    }
}