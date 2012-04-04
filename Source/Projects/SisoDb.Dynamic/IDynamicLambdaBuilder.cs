using System;
using System.Linq.Expressions;

namespace SisoDb.Dynamic
{
    public interface IDynamicLambdaBuilder 
    {
        IDynamicLambdaBuilderCache Cache { get; set; }
        LambdaExpression Build(Type type, string lambdaExpression);
    }
}