using System.Linq.Expressions;

namespace SisoDb.Querying.Lambdas
{
    public interface IExpressionEvaluator
    {
        object Evaluate(MethodCallExpression methodExpression);
        object Evaluate(MemberExpression memberExpression);
    }
}