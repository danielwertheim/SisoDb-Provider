using System.Linq.Expressions;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public interface IWhereParser
    {
        IParsedLambda Parse(LambdaExpression e);
    }
}