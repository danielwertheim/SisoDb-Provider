namespace SisoDb.Querying.Lambdas.Parsers
{
	public interface IExpressionParsers
	{
        IIncludeParser IncludeParser { get; set; }
        IWhereParser WhereParser { get; set; }
        IOrderByParser OrderByParser { get; set; }
	}
}