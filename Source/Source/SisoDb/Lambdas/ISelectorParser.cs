using System;
using System.Linq.Expressions;

namespace SisoDb.Lambdas
{
    internal interface ISelectorParser
    {
        IParsedLambda Parse<T>(Expression<Func<T, bool>> e) where T : class;
    }
}