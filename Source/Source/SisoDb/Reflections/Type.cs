using System;
using System.Linq.Expressions;
using SisoDb.Lambdas;

namespace SisoDb.Reflections
{
    public static class Type<T> where T : class
    {
        public static MemberExpression GetMemberExpression<TProp>(Expression<Func<T, TProp>> e)
        {
            return ExpressionTreeUtils.GetRightMostMember(e.Body);
        }
    }
}