using System;
using System.Linq.Expressions;
using SisoDb.NCore.Expressions;

namespace SisoDb.UnitTests
{
    internal static class Reflect<T> where T : class
    {
        internal static readonly Type Type = typeof(T);
        
        internal static Expression<Func<T, bool>> BoolExpressionFrom(Expression<Func<T, bool>> e)
        {
            return e;
        }

        internal static Expression<Func<T, TProp>> ExpressionFrom<TProp>(Expression<Func<T, TProp>> e)
        {
            return e;
        }

        internal static LambdaExpression LambdaFrom(Expression<Action<T>> e)
        {
            return e;
        }

        internal static LambdaExpression LambdaFrom<TProp>(Expression<Func<T, TProp>> e)
        {
            return e;
        }

        internal static MemberExpression MemberFrom<TProp>(Expression<Func<T, TProp>> e)
        {
            return e.Body.GetRightMostMember();
        }
    }
}