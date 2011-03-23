using System;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.Core;
using SisoDb.Resources;

namespace SisoDb.Querying.Lambdas
{
    public static class Expressions
    {
        public static bool IsNullConstant(Expression e)
        {
            var constant = e as ConstantExpression;
            if (constant == null)
                return false;

            return IsNullConstant(constant);
        }

        public static bool IsNullConstant(ConstantExpression e)
        {
            return e.Value == null || DBNull.Value.Equals(e.Value);
        }

        public static string ExtractRootObjectName(Expression e)
        {
            if (e.NodeType == ExpressionType.Lambda)
                return ((LambdaExpression)e).Parameters[0].Name;

            var path = e.ToString();
            return new string(path.TakeWhile(c => char.IsLetterOrDigit(c)).ToArray());
        }

        public static MemberExpression GetRightMostMember(Expression e)
        {
            if (e is LambdaExpression)
                return GetRightMostMember(((LambdaExpression)e).Body);

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