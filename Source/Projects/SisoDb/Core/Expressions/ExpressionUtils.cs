using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NCore;

namespace SisoDb.Core.Expressions
{
    public static class ExpressionUtils
    {
        public static bool ExpressionRepresentsNullValue(Expression e)
        {
            if (e is ConstantExpression)
                return (e as ConstantExpression).IsNullConstant();

            if (e is UnaryExpression && e.NodeType == ExpressionType.Convert)
            {
                var ue = (UnaryExpression)e;
                if (ue.Operand is ConstantExpression)
                    return (ue.Operand as ConstantExpression).IsNullConstant();
            }

            return false;
        }

        public static bool IsNullConstant(this ConstantExpression e)
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

            throw new SisoDbException("Don't know how to evaluate the expression type: '{0}'".Inject(e.GetType().Name));
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