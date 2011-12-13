using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using NCore.Expressions;
using NCore.Reflections;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Resources;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public class OrderByParser : IOrderByParser
	{
		public IParsedLambda Parse(OrderByExpression[] orderByExpressions)
		{
			Ensure.That(orderByExpressions, "orderByExpressions").HasItems();

			var nodesContainer = new NodesContainer();
			foreach (var orderByExpression in orderByExpressions.Where(e => e != null))
			{
				var memberExpression = orderByExpression.InnerLambdaExpression.Body.GetRightMostMember();
				var callExpression = (orderByExpression.InnerLambdaExpression.Body is UnaryExpression)
										 ? ((UnaryExpression)orderByExpression.InnerLambdaExpression.Body).Operand as MethodCallExpression
										 : orderByExpression.InnerLambdaExpression.Body as MethodCallExpression;
				
				if (callExpression != null)
					throw new SisoDbException(ExceptionMessages.OrderByParser_UnsupportedMethodForSortingDirection);

				var memberType = memberExpression.Type;
				if (memberType.IsEnumerableType())
					memberType = memberType.GetEnumerableElementType();

				var sortDirection = orderByExpression is OrderByAscExpression ? SortDirections.Asc : SortDirections.Desc;
				var sortingNode = new SortingNode(memberExpression.ToPath(), memberType, sortDirection);
				nodesContainer.AddNode(sortingNode);
			}

			return new ParsedLambda(nodesContainer.ToArray());
		}
	}
}