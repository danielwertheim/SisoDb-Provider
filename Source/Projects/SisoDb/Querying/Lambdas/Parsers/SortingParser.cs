using System;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using NCore.Expressions;
using NCore.Reflections;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Resources;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public class SortingParser : ISortingParser
    {
        public IParsedLambda Parse(LambdaExpression[] sortingExpressions)
        {
            Ensure.That(sortingExpressions, "sortingExpressions").HasItems();

            var nodesContainer = new NodesContainer();
            foreach (var lambda in sortingExpressions)
            {
                if (lambda == null)
                    continue;

                var memberExpression = lambda.Body.GetRightMostMember();

                var sortDirection = SortDirections.Asc;
                var callExpression = (lambda.Body is UnaryExpression)
                                         ? ((UnaryExpression)lambda.Body).Operand as MethodCallExpression
                                         : lambda.Body as MethodCallExpression;

                if (callExpression != null)
                {
                    switch (callExpression.Method.Name)
                    {
                        case "Asc":
                            sortDirection = SortDirections.Asc;
                            break;
                        case "Desc":
                            sortDirection = SortDirections.Desc;
                            break;
                        default:
                            throw new NotSupportedException(ExceptionMessages.SortingParser_UnsupportedMethodForSortingDirection);
                    }
                }

                var memberType = memberExpression.Type;
                if (memberType.IsEnumerableType())
                    memberType = memberType.GetEnumerableElementType();

                var sortingNode = new SortingNode(memberExpression.ToPath(), memberType, sortDirection);
                nodesContainer.AddNode(sortingNode);
            }

            return new ParsedLambda(nodesContainer.ToArray());
        }
    }
}