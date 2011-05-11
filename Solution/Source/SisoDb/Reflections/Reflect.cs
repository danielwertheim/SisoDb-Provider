using System;
using System.Linq.Expressions;
using SisoDb.Core.Expressions;
using SisoDb.Querying.Lambdas;

namespace SisoDb.Reflections
{
    public static class Reflect<T> where T : class
    {
        public static readonly Type Type = typeof(T);
        
        public static Expression<Func<T, bool>> BoolExpressionFrom(Expression<Func<T, bool>> e)
        {
            return e;
        }

        public static Expression<Func<T, TProp>> ExpressionFrom<TProp>(Expression<Func<T, TProp>> e)
        {
            return e;
        }

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
            return ExpressionUtils.GetRightMostMember(e.Body);
        }
    }
}