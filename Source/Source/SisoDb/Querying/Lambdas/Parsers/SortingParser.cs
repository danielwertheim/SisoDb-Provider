using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Core;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Resources;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public class SortingParser : ISortingParser
    {
        public IParsedLambda Parse(IEnumerable<LambdaExpression> sortingExpressions)
        {
            sortingExpressions.AssertHasItems("sortingExpressions");

            var nodesContainer = new NodesContainer();
            foreach (var lambda in sortingExpressions)
            {
                if (lambda == null)
                    continue;

                var memberExpression = Expressions.GetRightMostMember(lambda.Body);

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

                var sortingNode = new SortingNode(memberExpression.Path(), sortDirection);
                nodesContainer.AddNode(sortingNode);
            }

            return new ParsedLambda(nodesContainer);
        }
    }
}