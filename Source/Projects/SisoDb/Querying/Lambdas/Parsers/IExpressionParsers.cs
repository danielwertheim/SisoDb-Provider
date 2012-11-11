namespace SisoDb.Querying.Lambdas.Parsers
{
	public interface IExpressionParsers
	{
        IWhereParser WhereParser { get; set; }
        IOrderByParser OrderByParser { get; set; }
	}
}