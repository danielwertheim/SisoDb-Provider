using System;
using System.Linq.Expressions;

namespace SisoDb
{
    internal static class Expressions
    {
        internal static LambdaExpression LambdaFrom<TIn>(Expression<Func<TIn, bool>> e)
        {
            return e;
        }

        internal static MemberExpression MemberFrom<TIn, TOut>(this Expression<Func<TIn, TOut>> e)
        {
            return (MemberExpression)e.Body;
        }

        internal static bool IsNullConstant(Expression e)
        {
            var constant = e as ConstantExpression;
            if (constant == null)
                return false;

            return IsNullConstant(constant);
        }

        internal static bool IsNullConstant(ConstantExpression e)
        {
            return e.Value == null || DBNull.Value.Equals(e.Value);
        }
    }
}