using System;
using System.Linq.Expressions;

namespace SisoDb.Lambdas.Parsers
{
    public interface ISelectorParser
    {
        IParsedLambda Parse<T>(Expression<Func<T, bool>> e) where T : class;
    }
}