using System;
using System.Linq.Expressions;

namespace SisoDb
{
    public static class Expressions
    {
        public static LambdaExpression LambdaFrom<TIn>(Expression<Func<TIn, bool>> e)
        {
            return e;
        }

        public static MemberExpression MemberFrom<TIn, TOut>(this Expression<Func<TIn, TOut>> e)
        {
            return (MemberExpression)e.Body;
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
    }
}