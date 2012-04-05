using System;
using System.Linq.Expressions;

namespace SisoDb.Dynamic
{
    public interface IDynamicLambdaBuilderCache 
    {
        LambdaExpression GetOrAddExpression(string query, Func<LambdaExpression> fn);
    }
}