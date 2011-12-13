using System.Linq.Expressions;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public class WhereExpressionChainer
	{
		private LambdaExpression _innerLambdaExpression;

		public void Chain(LambdaExpression e)
		{
			if (_innerLambdaExpression == null)
			{
				_innerLambdaExpression = e;
				return;
			}

			var body = Expression.AndAlso(_innerLambdaExpression.Body, e.Body);
			_innerLambdaExpression = Expression.Lambda(body, _innerLambdaExpression.Parameters[0]);
		}

		public static implicit operator LambdaExpression(WhereExpressionChainer e)
		{
			return e._innerLambdaExpression;
		}
	}
}