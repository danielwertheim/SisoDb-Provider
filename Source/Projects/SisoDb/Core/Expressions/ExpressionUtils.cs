using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NCore;
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

		public static object Evaluate(this Expression e)
		{
			if (e is MethodCallExpression)
				return (e as MethodCallExpression).Evaluate();

			if (e is MemberExpression)
				return (e as MemberExpression).Evaluate();

			if (e is ConstantExpression)
				return (e as ConstantExpression).Evaluate();

		    if (e is NewArrayExpression)
		        return (e as NewArrayExpression).Evaluate();

		    throw new SisoDbException("Don't know how to evaluate the expression type: '{0}'".Inject(e.GetType().Name));
		}

        public static object[] Evaluate(this NewArrayExpression e)
        {
            return e.Expressions.Cast<ConstantExpression>().Select(ce => ce.Value).ToArray();
        }

        public static object Evaluate(this MethodCallExpression methodExpression)
		{
            if(methodExpression.Object == null)
            {
                var args = methodExpression.Arguments.OfType<ConstantExpression>().Select(c => c.Value).ToArray();
                if (args.Length == methodExpression.Arguments.Count)
                    return methodExpression.Method.Invoke(null, args);
            }

			return Expression.Lambda(methodExpression).Compile().DynamicInvoke();
		}

        public static object Evaluate(this MemberExpression memberExpression)
        {
            if (memberExpression.Member.MemberType == MemberTypes.Field && memberExpression.Expression is ConstantExpression)
            {
                var ce = (ConstantExpression)memberExpression.Expression;
                var obj = ce.Value;
                return obj == null
                    ? null
                    : ((FieldInfo)memberExpression.Member).GetValue(obj);
            }
            return Expression.Lambda(memberExpression).Compile().DynamicInvoke();
        }

		public static object Evaluate(this ConstantExpression constantExpression)
		{
			return constantExpression.Value;
		}
    }
}