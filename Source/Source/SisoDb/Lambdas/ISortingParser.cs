using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb.Lambdas
{
    public interface ISortingParser
    {
        IParsedLambda Parse(IEnumerable<LambdaExpression> sortingExpressions);
    }
}