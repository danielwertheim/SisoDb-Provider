using System.Linq.Expressions;

namespace SisoDb.Core.Expressions
{
    public interface IExpressionEvaluator
    {
        object Evaluate(MethodCallExpression methodExpression);
        object Evaluate(MemberExpression memberExpression);
    }
}