using System;
using System.Linq;
using System.Linq.Expressions;
using NCore.Reflections;

namespace SisoDb.Core.Expressions
{
    public static class ExpressionUtils
    {
		public static bool IsNullableValueTypeMemberExpression(this Expression e)
		{
			var memberExpression = e as MemberExpression;

			if (memberExpression == null)
				return false;

			return memberExpression.Type.IsNullablePrimitiveType() || memberExpression.Expression.Type.IsNullablePrimitiveType();
		}

    	public static Expression<Func<T, object>> GetMemberExpression<T>(string memberName) where T : class
		{
			var p = Expression.Parameter(typeof(T));
			var ma = Expression.MakeMemberAccess(p, typeof(T).GetMember(memberName)[0]);
			var conv = Expression.Convert(ma, typeof(object));
			var getter = Expression.Lambda(conv, p);

			return (Expression<Func<T, object>>)getter;
		}

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
            return new string(path.TakeWhile(char.IsLetterOrDigit).ToArray());
        }

		public static object Evaluate(this MethodCallExpression methodExpression)
		{
			return Expression.Lambda(methodExpression).Compile().DynamicInvoke();
		}

		public static object Evaluate(this MemberExpression memberExpression)
		{
			return Expression.Lambda(memberExpression).Compile().DynamicInvoke();
		}

		public static object Evaluate(this ConstantExpression constantExpression)
		{
			return constantExpression.Value;
		}

		public static T Evaluate<T>(this ConstantExpression constantExpression) where T : struct
		{
			return (T)constantExpression.Value;
		}
    }
}