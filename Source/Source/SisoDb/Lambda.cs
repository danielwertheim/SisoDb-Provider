using System;
using System.Linq.Expressions;
using SisoDb.Lambdas;

namespace SisoDb
{
    internal static class Type<T> where T : class
    {
        internal static MemberExpression GetMemberExpression<TProp>(Expression<Func<T, TProp>> e)
        {
            return ExpressionTreeUtils.GetRightMostMember(e.Body);
        }
    }
}