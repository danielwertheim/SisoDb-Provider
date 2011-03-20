using System.Linq.Expressions;

namespace SisoDb.Lambdas.Parsers
{
    public interface IWhereParser
    {
        IParsedLambda Parse(LambdaExpression e);
    }
}