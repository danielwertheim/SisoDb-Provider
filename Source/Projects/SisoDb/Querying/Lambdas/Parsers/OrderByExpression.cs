using System.Linq.Expressions;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public abstract class OrderByExpression : Expression
	{
		public readonly LambdaExpression InnerLambdaExpression;

		protected OrderByExpression(LambdaExpression lambdaExpression)
		{
			InnerLambdaExpression = lambdaExpression;
		}

		public static implicit operator LambdaExpression(OrderByExpression e)
		{
			return e.InnerLambdaExpression;
		}
	}
}