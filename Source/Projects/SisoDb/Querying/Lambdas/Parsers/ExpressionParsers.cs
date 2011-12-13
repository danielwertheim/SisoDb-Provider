namespace SisoDb.Querying.Lambdas.Parsers
{
	public class ExpressionParsers : IExpressionParsers
	{
		public ExpressionParsers()
		{
			IncludeParser = new IncludeParser();
			WhereParser = new WhereParser();
			OrderByParser = new OrderByParser();
		}

		public IIncludeParser IncludeParser { get; private set; }

		public IWhereParser WhereParser { get; private set; }

		public IOrderByParser OrderByParser { get; private set; }
	}
}