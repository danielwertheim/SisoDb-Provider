using System.Linq;
using System.Linq.Expressions;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.NCore.Expressions;
using SisoDb.NCore.Reflections;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Resources;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public class OrderByParser : IOrderByParser
	{
        protected readonly IDataTypeConverter DataTypeConverter;

        public OrderByParser(IDataTypeConverter dataTypeConverter)
        {
            Ensure.That(dataTypeConverter, "dataTypeConverter").IsNotNull();
            DataTypeConverter = dataTypeConverter;
        }

		public virtual IParsedLambda Parse(OrderByExpression[] orderByExpressions)
		{
			Ensure.That(orderByExpressions, "orderByExpressions").HasItems();

			var nodesContainer = new NodesCollection();
			foreach (var orderByExpression in orderByExpressions.Where(e => e != null))
			{
				var memberExpression = orderByExpression.InnerLambdaExpression.Body.GetRightMostMember();
                if (memberExpression == null)
                    throw new SisoDbException(ExceptionMessages.OrderByExpressionDoesNotTargetMember.Inject(orderByExpression.ToString()));

				var callExpression = (orderByExpression.InnerLambdaExpression.Body is UnaryExpression)
                    ? ((UnaryExpression)orderByExpression.InnerLambdaExpression.Body).Operand as MethodCallExpression
					: orderByExpression.InnerLambdaExpression.Body as MethodCallExpression;
				
				if (callExpression != null)
					throw new SisoDbException(ExceptionMessages.OrderByParser_UnsupportedMethodForSortingDirection);

				var memberType = memberExpression.Type;
				if (memberType.IsEnumerableType())
					memberType = memberType.GetEnumerableElementType();

				var sortDirection = orderByExpression is OrderByAscExpression ? SortDirections.Asc : SortDirections.Desc;
			    var memberPath = memberExpression.ToPath();
			    var sortingNode = new SortingNode(memberPath, memberType, DataTypeConverter.Convert(memberType, memberPath), sortDirection);
				nodesContainer.AddNode(sortingNode);
			}

			return new ParsedLambda(nodesContainer.ToArray());
		}
	}
}