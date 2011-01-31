using System.Linq;
using System.Linq.Expressions;
using SisoDb.Resources;

namespace SisoDb.Lambdas
{
    internal static class ExpressionTreeUtils
    {
        internal static string ExtractRootObjectName(Expression e)
        {
            if (e.NodeType == ExpressionType.Lambda)
                return ((LambdaExpression)e).Parameters[0].Name;

            var path = e.ToString();
            return new string(path.TakeWhile(c => char.IsLetterOrDigit(c)).ToArray());
        }

        internal static MemberExpression GetRightMostMember(Expression e)
        {
            if (e is MemberExpression)
                return (MemberExpression)e;

            if (e is MethodCallExpression)
                return GetRightMostMember(((MethodCallExpression)e).Object);

            throw new SisoDbException(ExceptionMessages.ExpressionUtils_GetRightMostMember_NoMemberFound
                .Inject(e.ToString()));
        }
    }
}