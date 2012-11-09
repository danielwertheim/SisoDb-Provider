namespace SisoDb.Querying.Lambdas.Parsers
{
	public interface IExpressionParsers
	{
        //TODO: Rem for v16.0.0 final
        //IIncludeParser IncludeParser { get; set; }
        IWhereParser WhereParser { get; set; }
        IOrderByParser OrderByParser { get; set; }
	}
}