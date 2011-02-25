using System;
using System.Linq.Expressions;
using SisoDb.Lambdas;

namespace SisoDb.Reflections
{
    public static class Reflect<T> where T : class
    {
        public static LambdaExpression LambdaFrom(Expression<Action<T>> e)
        {
            return e;
        }

        public static LambdaExpression LambdaFrom<TProp>(Expression<Func<T, TProp>> e)
        {
            return e;
        }

        public static MemberExpression MemberFrom<TProp>(Expression<Func<T, TProp>> e)
        {
            return Expressions.GetRightMostMember(e.Body);
        }
    }
}