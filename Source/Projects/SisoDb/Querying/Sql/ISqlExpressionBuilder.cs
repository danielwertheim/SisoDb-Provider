using SisoDb.Querying.Lambdas;

namespace SisoDb.Querying.Sql
{
    /// <summary>
    /// Converts <see cref="IParsedLambda"/> representing 
    /// Wheres, Sortings, Paging etc. to an <see cref="ISqlExpression"/>.
    /// </summary>
    public interface ISqlExpressionBuilder
    {
        ISqlExpression Process(IQuery query);
    }
}