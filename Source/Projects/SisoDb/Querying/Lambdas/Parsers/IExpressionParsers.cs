namespace SisoDb.Querying.Lambdas.Parsers
{
	public interface IExpressionParsers
	{
		IIncludeParser IncludeParser { get; }
		IWhereParser WhereParser { get; }
		IOrderByParser OrderByParser { get; }
	}
}