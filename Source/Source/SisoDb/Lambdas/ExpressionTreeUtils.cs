using System.Linq;
using System.Linq.Expressions;
using SisoDb.Resources;

namespace SisoDb.Lambdas
{
    public static class ExpressionTreeUtils
    {
        public static string ExtractRootObjectName(Expression e)
        {
            if (e.NodeType == ExpressionType.Lambda)
                return ((LambdaExpression)e).Parameters[0].Name;

            var path = e.ToString();
            return new string(path.TakeWhile(c => char.IsLetterOrDigit(c)).ToArray());
        }

        public static MemberExpression GetRightMostMember(Expression e)
        {
            if (e is MemberExpression)
                return (MemberExpression)e;

            if (e is MethodCallExpression)
            {
                var callExpression = (MethodCallExpression) e;
                var member = callExpression.Arguments.Count > 0 ? callExpression.Arguments[0] : callExpression.Object;
                return GetRightMostMember(member);
            }

            if(e is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression) e;
                return GetRightMostMember(unaryExpression.Operand);
            }

            throw new SisoDbException(ExceptionMessages.ExpressionUtils_GetRightMostMember_NoMemberFound
                .Inject(e.ToString()));
        }
    }
}