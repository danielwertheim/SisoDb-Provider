using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using SisoDb.Resources;

namespace SisoDb.Lambdas
{
    public class ExpressionEvaluator : IExpressionEvaluator
    {
        public object Evaluate(MethodCallExpression methodExpression)
        {
            var args = methodExpression.Arguments.Count < 1
                           ? null
                           : GetArgs(methodExpression.Arguments);

            if (methodExpression.Method.IsStatic)
                return methodExpression.Method.Invoke(null, args);

            return Expression.Lambda(methodExpression).Compile().DynamicInvoke(); //.DynamicInvoke(args);
        }

        private object[] GetArgs(IEnumerable<Expression> expressionArgs)
        {
            var args = new List<object>();

            foreach (var arg in expressionArgs)
            {
                if (arg.NodeType == ExpressionType.Constant)
                    args.Add(((ConstantExpression)arg).Value);
                else if (arg.NodeType == ExpressionType.Call)
                {
                    var tmp = Evaluate((MethodCallExpression)arg);
                    if (tmp != null)
                        args.Add(tmp);
                }
                else if (arg.NodeType == ExpressionType.MemberAccess)
                {
                    var tmp = Evaluate((MemberExpression)arg);
                    if (tmp != null)
                        args.Add(tmp);
                }
            }

            return args.Count < 1 ? null : args.ToArray();
        }

        public object Evaluate(MemberExpression memberExpression)
        {
            var constant = (ConstantExpression)memberExpression.Expression;
            var member = memberExpression.Member;

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return EvaluateMemberAccessField(constant, (FieldInfo)member);
                case MemberTypes.Property:
                    return EvaluateMemberAccessProperty(constant, (PropertyInfo)member);
                default:
                    throw new NotSupportedException(
                        ExceptionMessages.ExpressionEvaluator_EvaluateMemberExpression_NotSupported
                        .Inject(member.Name, member.MemberType));
            }
        }

        private static object EvaluateMemberAccessProperty(ConstantExpression node, PropertyInfo propertyAccessor)
        {
            var prop = node == null ? null : node.Value;
            var value = propertyAccessor.GetValue(prop, null);

            return ConvertMemberAccessValue(value);
        }

        private static object EvaluateMemberAccessField(ConstantExpression node, FieldInfo fieldAccessor)
        {
            object value;

            if (fieldAccessor.IsStatic)
                value = fieldAccessor.GetValue(null);
            else
                value = node == null ? null : fieldAccessor.GetValue(node.Value);

            return ConvertMemberAccessValue(value);
        }

        private static object ConvertMemberAccessValue(object value)
        {
            return DBNull.Value.Equals(value) ? null : value;
        }
    }
}