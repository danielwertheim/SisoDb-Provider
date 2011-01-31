using System;
using System.Linq.Expressions;

namespace SisoDb.Lambdas
{
    internal interface ILambdaParser
    {
        IParsedLambda Parse<T>(Expression<Func<T, bool>> e);
    }
}