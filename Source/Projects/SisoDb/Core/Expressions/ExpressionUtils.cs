using System;
using System.Linq;
using System.Linq.Expressions;

namespace SisoDb.Core.Expressions
{
    public static class ExpressionUtils
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
            return new string(path.TakeWhile(char.IsLetterOrDigit).ToArray());
        }
    }
}