using System;
using System.Linq.Expressions;

namespace SisoDb.Dynamic
{
    public static class ExpressionFactory
    {
        public static Expression<Func<T, bool>> CreatePredicate<T>(Expression<Func<T, bool>> e)
        {
            return e;
        }

        public static Expression<Func<T, object>> CreateMember<T>(Expression<Func<T, object>> e)
        {
            return e;
        }
    }
}