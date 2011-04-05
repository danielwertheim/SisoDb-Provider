using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Core;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Structures.Schemas;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public class IncludeParser : IIncludeParser
    {
        public IParsedLambda Parse<TInclude>(IEnumerable<LambdaExpression> includeExpressions) where TInclude : class
        {
            includeExpressions.AssertHasItems("includeExpressions");

            var nodes = new NodesContainer();

            foreach (var includeExpression in includeExpressions)
            {
                var childStructureName = StructureType<TInclude>.Instance.Name;
                var memberExpression = Expressions.GetRightMostMember(includeExpression);
                var idReferencePath = memberExpression.Path();
                var objectReferencePath = BuildObjectReferencePath(idReferencePath);
                
                var includeNode = new IncludeNode(childStructureName, idReferencePath, objectReferencePath);
                nodes.AddNode(includeNode);    
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