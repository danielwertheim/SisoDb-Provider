using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public interface ISortingParser
    {
        IParsedLambda Parse(IEnumerable<LambdaExpression> sortingExpressions);
    }
}