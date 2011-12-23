using System.Linq.Expressions;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public class OrderByDescExpression : OrderByExpression
	{
		public OrderByDescExpression(LambdaExpression lambdaExpression) : base(lambdaExpression) { }
	}
}