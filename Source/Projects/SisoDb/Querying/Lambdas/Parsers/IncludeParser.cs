using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Core;
using SisoDb.Core.Expressions;
using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public class IncludeParser : IIncludeParser
    {
        public IParsedLambda Parse(string includedStructureTypeName, IEnumerable<LambdaExpression> includeExpressions)
        {
            includeExpressions.AssertHasItems("includeExpressions");

            var nodes = new NodesContainer();

            foreach (var includeExpression in includeExpressions)
            {
                var memberExpression = ExpressionUtils.GetRightMostMember(includeExpression);
                var idReferencePath = memberExpression.Path();
                var objectReferencePath = BuildObjectReferencePath(idReferencePath);

                nodes.AddNode(
                    new IncludeNode(includedStructureTypeName, idReferencePath, objectReferencePath));    
            }

            return new ParsedLambda(nodes);
        }

        private static string BuildObjectReferencePath(string idReferencePath)
        {
            return !idReferencePath.EndsWith("Id") 
                ? idReferencePath 
                : idReferencePath.Substring(0, idReferencePath.Length - 2);
        }
    }
}