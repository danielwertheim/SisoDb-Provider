using System.Linq.Expressions;

namespace SisoDb.Core.Expressions
{
    public class LambdaExpressionEvaluator : IExpressionEvaluator
    {
        public object Evaluate(MethodCallExpression methodExpression)
        {
            return Expression.Lambda(methodExpression).Compile().DynamicInvoke();
        }

        public object Evaluate(MemberExpression memberExpression)
        {
            return Expression.Lambda(memberExpression).Compile().DynamicInvoke();
        }
    }
}