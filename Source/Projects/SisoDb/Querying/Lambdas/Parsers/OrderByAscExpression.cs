using System.Linq.Expressions;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public class OrderByAscExpression : OrderByExpression
	{
		public OrderByAscExpression(LambdaExpression lambdaExpression) : base(lambdaExpression) {}
	}
}