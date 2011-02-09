using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb.Lambdas
{
    internal interface ISortingParser
    {
        IParsedLambda Parse(IEnumerable<LambdaExpression> sortingExpressions);
    }
}