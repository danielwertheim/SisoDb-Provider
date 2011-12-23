namespace SisoDb.Querying.Lambdas.Parsers
{
	public interface IOrderByParser
	{
		IParsedLambda Parse(OrderByExpression[] orderByExpressions);
	}
}