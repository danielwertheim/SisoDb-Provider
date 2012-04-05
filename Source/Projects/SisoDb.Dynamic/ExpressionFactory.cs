using System;
using System.Linq.Expressions;

namespace SisoDb.Dynamic
{
    public static class ExpressionFactory
    {
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> e)
        {
            return e;
        }
    }
}