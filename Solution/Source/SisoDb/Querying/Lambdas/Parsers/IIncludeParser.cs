using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public interface IIncludeParser
    {
        IParsedLambda Parse(string includedStructureTypeName, IEnumerable<LambdaExpression> includeExpressions);
    }
}