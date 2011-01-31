using System.Linq.Expressions;

namespace SisoDb.Lambdas
{
    internal interface IExpressionEvaluator
    {
        object Evaluate(MethodCallExpression methodExpression);
        object Evaluate(MemberExpression memberExpression);
    }
}