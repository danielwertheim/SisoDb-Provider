using System.Linq.Expressions;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public interface ISortingParser
    {
        IParsedLambda Parse(LambdaExpression[] sortingExpressions);
    }
}